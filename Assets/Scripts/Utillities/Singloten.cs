using UnityEngine;

public class Singloten<T> : MonoBehaviour where T : Singloten<T>
{
    private static T instance;

    public static T Instance
    {
        get => instance;
    }

    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }
}
    
