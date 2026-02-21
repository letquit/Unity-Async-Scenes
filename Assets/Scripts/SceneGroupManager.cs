using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SceneManagement
{
    /// <summary>
    /// 场景组管理器，用于加载和卸载场景组。
    /// </summary>
    public class SceneGroupManager
    {
        /// <summary>
        /// 当场景加载完成时触发的事件。
        /// </summary>
        public event Action<string> OnSceneLoaded = delegate { };

        /// <summary>
        /// 当场景卸载完成时触发的事件。
        /// </summary>
        public event Action<string> OnSceneUnloaded = delegate { };

        /// <summary>
        /// 当整个场景组加载完成时触发的事件。
        /// </summary>
        public event Action OnSceneGroupLoaded = delegate { };

        /// <summary>
        /// 当前激活的场景组。
        /// </summary>
        private SceneGroup ActiveSceneGroup;

        /// <summary>
        /// 异步加载指定的场景组。
        /// </summary>
        /// <param name="group">要加载的场景组。</param>
        /// <param name="progress">用于报告加载进度的进度对象。</param>
        /// <param name="reloadDupScenes">是否重新加载已存在的场景，默认为false。</param>
        /// <returns>表示异步操作的任务。</returns>
        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            // 卸载当前所有场景
            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            // 收集当前已加载的场景名称
            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;

            // 创建异步操作组以跟踪所有加载操作
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                // 如果不允许重复加载且场景已存在，则跳过
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name)) continue;

                // 异步加载场景
                var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);

                // 模拟延迟
                // await Task.Delay(TimeSpan.FromSeconds(2.5f));   //TODO：移除

                operationGroup.Operations.Add(operation);

                // 触发场景加载事件
                OnSceneLoaded.Invoke(sceneData.Name);
            }

            // 等待所有异步操作完成，并报告进度
            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(100);
            }

            // 设置活动场景
            Scene activeScene =
                SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            // 触发场景组加载完成事件
            OnSceneGroupLoaded.Invoke();
        }

        /// <summary>
        /// 异步卸载当前所有非必要场景。
        /// </summary>
        /// <returns>表示异步操作的任务。</returns>
        public async Task UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;

            // 收集需要卸载的场景名称
            for (var i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                // 跳过活动场景和启动场景
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;
                scenes.Add(sceneName);
            }

            // 创建异步操作组以跟踪所有卸载操作
            var operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                // 触发场景卸载事件
                OnSceneUnloaded.Invoke(scene);
            }

            // 等待所有异步操作完成
            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);  // 避免紧密循环
            }

            // 可选：卸载未使用的资源
            await Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 表示一组异步操作的结构体，用于跟踪多个场景加载或卸载操作的进度和状态。
    /// </summary>
    public readonly struct AsyncOperationGroup
    {
        /// <summary>
        /// 包含所有异步操作的列表。
        /// </summary>
        public readonly List<AsyncOperation> Operations;

        /// <summary>
        /// 获取所有操作的平均进度。
        /// </summary>
        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);

        /// <summary>
        /// 检查所有操作是否已完成。
        /// </summary>
        public bool IsDone => Operations.All(o => o.isDone);

        /// <summary>
        /// 初始化一个新的异步操作组实例。
        /// </summary>
        /// <param name="initialCapacity">初始容量，用于预分配列表空间。</param>
        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}