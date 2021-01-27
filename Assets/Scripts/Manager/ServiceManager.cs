using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : SingletonMonoBehaviour<ServiceManager>
{
    public NetworkSystem networkSystem;
    public PlayerManager playerManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
