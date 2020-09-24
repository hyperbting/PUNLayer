using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public NetworkSystem networkSystem;
    public PlayerManager playerManager;
    public InteractableManager interactableManager;

    static ServiceManager instance;
    public static ServiceManager Instance {
        get {
            if (instance == null)
                instance = GameObject.Find("ServiceManager").GetComponent<ServiceManager>();

            return instance;
        }
    }
}
