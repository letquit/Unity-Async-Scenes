using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// PersistentSingleton 是一个泛型单例类，继承自 MonoBehaviour。
/// 它确保在 Unity 场景中只有一个指定类型的实例存在，并且该实例在场景切换时不会被销毁。
/// </summary>
/// <typeparam name="T">必须是 Component 的子类，表示单例的具体类型。</typeparam>
public class PersistentSingleton<T> : MonoBehaviour where T : Component {
    /// <summary>
    /// 标题属性，用于在 Inspector 中显示标题。
    /// </summary>
    [Title("Persistent Singleton")]
    [Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
    public bool UnparentOnAwake = true;

    /// <summary>
    /// 静态属性，检查是否存在当前类型的实例。
    /// </summary>
    public static bool HasInstance => instance != null;

    /// <summary>
    /// 静态属性，获取当前实例。
    /// </summary>
    public static T Current => instance;

    /// <summary>
    /// 静态字段，存储当前类型的唯一实例。
    /// </summary>
    protected static T instance;

    /// <summary>
    /// 静态属性，获取或创建当前类型的实例。
    /// 如果实例不存在，则尝试查找场景中的第一个匹配对象；
    /// 如果找不到，则自动创建一个新的 GameObject 并添加组件。
    /// </summary>
    public static T Instance {
        get {
            // 如果实例为空，尝试查找场景中的第一个匹配对象
            if (instance == null) {
                instance = FindFirstObjectByType<T>();
                // 如果仍然找不到，创建新的 GameObject 并添加组件
                if (instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name + "AutoCreated";
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// Awake 生命周期方法，在对象初始化时调用。
    /// 调用 InitializeSingleton 方法进行单例初始化。
    /// </summary>
    protected virtual void Awake() => InitializeSingleton();

    /// <summary>
    /// 初始化单例逻辑。
    /// 包括设置父对象、确保唯一性以及标记对象不随场景销毁。
    /// </summary>
    protected virtual void InitializeSingleton() {
        // 如果不在运行时环境中，直接返回
        if (!Application.isPlaying) {
            return;
        }

        // 如果启用 UnparentOnAwake，则将当前对象从父对象中分离
        if (UnparentOnAwake) {
            transform.SetParent(null);
        }

        // 如果当前没有实例，则将自己设为实例并标记为不销毁
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(transform.gameObject);
            enabled = true;
        } else {
            // 如果已有实例且不是当前对象，则销毁当前对象
            if (this != instance) {
                Destroy(this.gameObject);
            }
        }
    }
}
