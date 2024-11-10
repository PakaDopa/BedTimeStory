using UnityEngine;
using System;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool _shuttingDown = false;
    private static object _lock = new object();
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindAnyObjectByType(typeof(T));
                    
                    if (_instance == null)
                    {
                        var singletonObj = new GameObject();
                        _instance = singletonObj.AddComponent<T>();
                        singletonObj.name = typeof(T).ToString() + "(Singleton)";

                        DontDestroyOnLoad(singletonObj);
                    }
                    // else
                    // {
                    //     DontDestroyOnLoad(_instance.gameObject);
                    // }
                }
            }

            return _instance;
        }
    }

    private void OnApplicationQuit()
    {
        _shuttingDown = true;
    }

    private void OnDestroy()
    {
        _shuttingDown = true;
    }

    //MainScene ������ �ݵ�� Init(Awake)�� �ʿ��� 
    public abstract void Init();
}