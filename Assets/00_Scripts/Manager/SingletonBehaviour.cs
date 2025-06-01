using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : Component
{
    protected bool isDontDestroyOnLoad = true;
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindAnyObjectByType(typeof(T));
                if (instance == null)
                {
                    SetupInstance();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        RemoveDuplicates();
    }
    
    protected virtual void OnDestroy()
    {
        instance = null;
    }

    /// <summary>
    /// 인스턴스 생성
    /// </summary>
    private static void SetupInstance()
    {
        var go = new GameObject
        {
            name = typeof(T).Name
        };
        instance = go.AddComponent<T>();
    }

    /// <summary>
    /// 중복 인스턴스 제거 및 DontDestroyOnLoad 설정
    /// </summary>
    private void RemoveDuplicates()
    {
        if (instance == null)
        {
            instance = this as T;
            if (isDontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}