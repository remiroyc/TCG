using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get { return _instance ?? (_instance = Object.FindObjectOfType<T>()); }
    }

}
