using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindFirstObjectByType<T>();
            if (_instance != null) return _instance;

            return _instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }

        private set
        {
            if (_instance == null)
            {
                _instance = value;
                DontDestroyOnLoad(_instance.gameObject);
            }
            else if (_instance != value)
            {
                DestroyImmediate(value.gameObject);
            }
        }
    }

    protected virtual void Awake()
    {
        Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this) { _instance = null; }
    }

    public static bool IsInitialized()
    {
        return _instance != null;
    }
}
