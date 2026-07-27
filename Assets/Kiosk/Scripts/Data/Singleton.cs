using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Yeop
{
    //SingletonAttribute 설정 사용
    //씬 내 1개 인스턴스 관리
    //필요 시 자동 생성 가능
    public static class Singleton<T> where T : MonoBehaviour
    {
        static Singleton()
        {
            awoken = new HashSet<T>();

            attribute = (SingletonAttribute)System.Attribute.GetCustomAttribute(
                typeof(T),
                typeof(SingletonAttribute)
            );

            if (attribute == null)
            {
                throw new UnityException($"Missing singleton attribute for '{typeof(T)}'.");
            }
        }

        private static readonly SingletonAttribute attribute;

        private static bool persistent => attribute.Persistent;
        private static bool automatic => attribute.Automatic;
        private static string name => attribute.Name;
        private static HideFlags hideFlags => attribute.HideFlags;

        private static readonly object _lock = new object();
        private static readonly HashSet<T> awoken;

        private static T _instance;

        public static bool instantiated
        {
            get
            {
                lock (_lock)
                {
                    if (Application.isPlaying)
                    {
                        return _instance != null;
                    }
                    else
                    {
                        return FindInstances().Length == 1;
                    }
                }
            }
        }

        public static T instance
        {
            get
            {
                lock (_lock)
                {
                    if (Application.isPlaying)
                    {
                        if (_instance == null)
                        {
                            Instantiate();
                        }

                        return _instance;
                    }
                    else
                    {
                        return Instantiate();
                    }
                }
            }
        }

        private static T[] FindObjectsOfType()
        {
#if UNITY_2023_1_OR_NEWER
            return UnityObject.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return UnityObject.FindObjectsOfType<T>();
#endif
        }

        private static T[] FindInstances()
        {
            return FindObjectsOfType();
        }

        public static T Instantiate()
        {
            lock (_lock)
            {
                var instances = FindInstances();

                if (instances.Length == 1)
                {
                    _instance = instances[0];
                }
                else if (instances.Length == 0)
                {
                    if (automatic)
                    {
                        GameObject singletonObject = new GameObject(name ?? typeof(T).Name);
                        singletonObject.hideFlags = hideFlags;

                        T createdInstance = singletonObject.AddComponent<T>();
                        createdInstance.hideFlags = hideFlags;

                        Awake(createdInstance);

                        if (persistent && Application.isPlaying)
                        {
                            UnityObject.DontDestroyOnLoad(singletonObject);
                        }
                    }
                    else
                    {
                        throw new UnityException($"Missing '{typeof(T)}' singleton in the scene.");
                    }
                }
                else
                {
                    throw new UnityException($"More than one '{typeof(T)}' singleton in the scene.");
                }

                return _instance;
            }
        }

        public static void Awake(T instance)
        {
            if (instance == null)
            {
                throw new UnityException("Singleton Awake instance is null.");
            }

            if (awoken.Contains(instance))
            {
                return;
            }

            if (_instance != null && _instance != instance)
            {
                throw new UnityException($"More than one '{typeof(T)}' singleton in the scene.");
            }

            _instance = instance;
            awoken.Add(instance);
        }

        public static void OnDestroy(T instance)
        {
            if (instance == null)
            {
                throw new UnityException("Singleton OnDestroy instance is null.");
            }

            if (_instance == instance)
            {
                _instance = null;
            }
        }
    }
}