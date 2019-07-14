using System;
using System.Linq;

namespace UnityEngine.Extension
{
    /// <summary>
    /// SingletonMonoBehaviourのオプション
    /// シーン移動時に削除しないオプション
    /// </summary>
    public interface ISingletonMonoBehaviourAutoDontDestroyOnLoad { }
    /// <summary>
    /// SingletonMonoBehaviourのオプション
    /// 自動的に良しなに生成してくれる
    /// </summary>
    public interface ISingletonMonoBehaviourAutoCreate { }
    /// <summary>
    /// SingletonMonoBehaviourのオプション
    /// </summary>
    public interface ISingletonMonoBehaviourAutoInitialize {
        bool IsInitialized { get; }
        void Initialize();
    }

    /// <summary>
    /// Uniqueなオブジェクトを簡単に作成
    /// </summary>
    /// <typeparam name="T">このクラスを継承するオブジェクト本体</typeparam>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static Type[] _options = null;
        /// <summary>
        /// OptionInterfaceのキャシュ
        /// </summary>
        private static Type[] Options
        {
            get
            {
                if (_options == null)
                    _options = typeof(T).GetInterfaces();
                return _options;
            }
        }


        /// <summary>
        /// Instanceを保持するクラス
        /// </summary>
        private static T _instance = null;

        /// <summary>
        /// Instanceを取得する
        /// 最初にFindObjectTypeを一度実行
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    // 自動生成オプションが入っているか
                    if (Options.Any(i => i == typeof(ISingletonMonoBehaviourAutoCreate)))
                        _instance = new GameObject(typeof(T).FullName).AddComponent<T>();
                }
                if(_instance != null)
                {
                    if(Options.Any(i => i == typeof(ISingletonMonoBehaviourAutoInitialize)))
                    {
                        ISingletonMonoBehaviourAutoInitialize initialize = (ISingletonMonoBehaviourAutoInitialize)_instance;
                        if (!initialize.IsInitialized)
                            initialize.Initialize();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Instanceで取得可能なユークオブジェクトか
        /// 違うならオブジェクトを削除
        /// </summary>
        protected virtual void SingletonInitialize()
        {
            if (Instance == this)
            {
                // DontDestroy Optionが入っている場合
                if (Options.Any(i => i == typeof(ISingletonMonoBehaviourAutoDontDestroyOnLoad)))
                    DontDestroyOnLoad(this);
                return;
            }
            Destroy(this);
        }

        /// <summary>
        /// AwakeでSingletonInitializeを実行
        /// InstanceのUniqueを保つ
        /// </summary>
        protected virtual void Awake()
        {
            SingletonInitialize();
        }
    }
}