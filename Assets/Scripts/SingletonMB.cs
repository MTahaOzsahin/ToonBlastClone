using UnityEngine;

public class SingletonMB<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static readonly object _lock = new object();

    private static bool _isQuitting = false;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }

                    if (_instance == null && !_isQuitting) return null;
                }

                return _instance;
            }
        }
    }
    
    public void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}