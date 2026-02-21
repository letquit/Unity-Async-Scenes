using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootstrapper 类用于在游戏启动时初始化核心逻辑。
/// 该类继承自 PersistentSingleton，确保全局唯一实例。
/// </summary>
public class Bootstrapper : PersistentSingleton<Bootstrapper>
{
    /// <summary>
    /// 在场景加载之前异步初始化游戏核心逻辑。
    /// 该方法通过 RuntimeInitializeOnLoadMethod 特性在运行时自动调用。
    /// 主要功能是加载名为 "Bootstrapper" 的场景，并确保以单场景模式加载。
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void Init()
    {
        // 输出日志，标识 Bootstrapper 初始化开始
        Debug.Log("Bootstrapper...");

        // 异步加载 "Bootstrapper" 场景，使用单场景模式替换当前场景
        await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
    }
}