using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSystem : MonoBehaviour, INetworkConnectUser
{
    public static NetworkSystem Instance;

    public GameObject INetworkConnectGO;
    public INetworkConnect inc;

    [Space]
    [Header("Debug")]
    public GameObject DebugUI;
    #region OnEvent
    public Action<GameObject> OnJoinedRoomEvent { get; set; }
    #endregion

    private void Awake()
    {
        Instance = this;
        inc = INetworkConnectGO.GetComponent<INetworkConnect>();
        inc.Init (this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Debug.isDebugBuild)
            DebugUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
