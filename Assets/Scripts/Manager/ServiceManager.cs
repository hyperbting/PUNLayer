using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public NetworkSystem networkSystem;
    public PlayerManager playerManager;

    static ServiceManager instance;
    public static ServiceManager Instance {
        get {
            if (instance == null)
            {
                var go = GameObject.Find("ServiceManager");
                if(go != null)
                    instance = go.GetComponent<ServiceManager>();
            }

            return instance;
        }
    }

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
