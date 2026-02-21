using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    /// <summary>
    /// 场景加载器类，负责管理场景组的加载和卸载，并提供加载进度的可视化反馈。
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Image loadingBar; // 加载进度条图像组件
        [SerializeField] private float fillSpeed = 0.5f; // 进度条填充速度
        [SerializeField] private Canvas loadingCanvas; // 加载界面画布
        [SerializeField] private Camera loadingCamera; // 加载界面相机
        [SerializeField] private SceneGroup[] sceneGroups; // 场景组数组

        private float targetProgress; // 目标加载进度
        private bool isLoading; // 是否正在加载场景
        
        public readonly SceneGroupManager manager = new SceneGroupManager(); // 场景组管理器实例

        /// <summary>
        /// 在Awake阶段注册场景加载和卸载事件的回调函数。
        /// </summary>
        private void Awake()
        {
            manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        /// <summary>
        /// 在Start阶段异步加载第一个场景组。
        /// </summary>
        private async void Start()
        {
            await LoadSceneGroup(0);
        }

        /// <summary>
        /// 每帧更新加载进度条的显示效果。
        /// </summary>
        private void Update()
        {
            if (!isLoading) return;

            float currentFillAmount = loadingBar.fillAmount;
            float progressdifference = Math.Abs(currentFillAmount - targetProgress);
            
            float dynamicFillSpeed = fillSpeed * progressdifference;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        /// <summary>
        /// 异步加载指定索引的场景组。
        /// </summary>
        /// <param name="index">要加载的场景组索引。</param>
        /// <returns>表示异步操作的任务。</returns>
        public async Task LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;
            
            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }
            
            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
            
            EnableLoadingCanvas();
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        /// <summary>
        /// 启用或禁用加载界面画布和相机。
        /// </summary>
        /// <param name="enable">是否启用加载界面，默认为true。</param>
        private void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }

    /// <summary>
    /// 加载进度类，实现IProgress<float>接口，用于报告加载进度。
    /// </summary>
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed; // 进度更新事件

        private const float ratio = 1f; // 进度比例因子
        
        /// <summary>
        /// 报告当前加载进度。
        /// </summary>
        /// <param name="value">当前进度值（0到1之间）。</param>
        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}