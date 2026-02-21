using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;
using Unity.VectorGraphics;

namespace Systems.SceneManagement
{
    /// <summary>
    /// 表示一个场景组，包含场景组名称和场景列表。
    /// </summary>
    [Serializable]
    public class SceneGroup
    {
        /// <summary>
        /// 场景组的名称，默认值为 "New Scene Group"。
        /// </summary>
        public string GroupName = "New Scene Group";

        /// <summary>
        /// 场景组中包含的场景数据列表。
        /// </summary>
        public List<SceneData> Scenes;

        /// <summary>
        /// 根据指定的场景类型查找场景名称。
        /// </summary>
        /// <param name="sceneType">要查找的场景类型。</param>
        /// <returns>匹配的场景名称；如果未找到匹配项，则返回 null。</returns>
        public string FindSceneNameByType(SceneType sceneType)
        {
            // 使用 LINQ 查找第一个匹配指定场景类型的场景，并返回其名称。
            return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType)?.Reference.Name;
        }
    }

    /// <summary>
    /// 表示单个场景的数据结构，包含场景引用和场景类型。
    /// </summary>
    [Serializable]
    public class SceneData
    {
        /// <summary>
        /// 场景的引用对象。
        /// </summary>
        public SceneReference Reference;

        /// <summary>
        /// 获取场景的名称，通过场景引用的 Name 属性获取。
        /// </summary>
        public string Name => Reference.Name;

        /// <summary>
        /// 场景的类型，用于标识场景的用途。
        /// </summary>
        public SceneType SceneType;
    }

    /// <summary>
    /// 定义场景类型的枚举，用于区分不同用途的场景。
    /// </summary>
    public enum SceneType
    {
        /// <summary>
        /// 活动场景。
        /// </summary>
        ActiveScene,

        /// <summary>
        /// 主菜单场景。
        /// </summary>
        MainMenu,

        /// <summary>
        /// 用户界面场景。
        /// </summary>
        UserInterface,

        /// <summary>
        /// 头戴显示器（HUD）场景。
        /// </summary>
        HUD,

        /// <summary>
        /// 过场动画场景。
        /// </summary>
        Cinematic,

        /// <summary>
        /// 环境场景。
        /// </summary>
        Environment,

        /// <summary>
        /// 工具场景。
        /// </summary>
        Tooling
    }
}