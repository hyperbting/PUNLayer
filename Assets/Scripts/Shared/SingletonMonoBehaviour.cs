using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get {
            if (instance == null)
            {
                // Find the active singleton already created and count
                T[] objectResult = FindObjectsOfType<T>();
                Debug.Log("DEBUG: GenericMonoSingleton FindObjectsOfType.count=" + objectResult.Length);

                // Find the ACTIVE singleton already created: https://docs.unity3d.com/ScriptReference/Object.FindObjectOfType.html
                instance = (T)Object.FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    Debug.LogError("DEBUG: GenericMonoSingleton Cannot Found In Scene");

                    //instance = CreateSingleton();
                    var SGO = new GameObject($"{typeof(T).Name}(Singleton)");
                    instance = SGO.AddComponent<T>();

                    DontDestroyOnLoad(SGO); // Make instance persistent.
                }
            }

            Debug.Log("DEBUG: GenericMonoSingleton [case2]: Found the active object in memory");
            return instance;
        }
    }

    //private static T CreateSingleton()
    //{
    //    var SGO = new GameObject($"{typeof(T).Name}(Singleton)");
    //    var instance = SGO.AddComponent<T>();

    //    // Make instance persistent.
    //    DontDestroyOnLoad(SGO);
    //    return instance;
    //}
}
