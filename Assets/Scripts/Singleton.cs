public abstract class Singleton<T> where T : Singleton<T>, new()
{
    protected Singleton()
    {
    }

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Init();
            }
            return _instance;
        }
    }

    protected abstract void Init();
}