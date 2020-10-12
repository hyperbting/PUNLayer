using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSystem : MonoBehaviour, INetworkConnectUser
{
    [Header("NetworkConnect is the network core")]
    [Tooltip("Implement interface of Network Abilities")]
    public GameObject INetworkConnectGO;

    [Header("SyncHandler Maintain Properties Sync")]
    [Tooltip("Automatically create token when OnJoinedRoom/ CreatedAlreadyInRoom")]
    public GameObject NetworkSyncHandler;

    INetworkConnect inc;

    [Space]
    [Header("Debug")]
    public GameObject DebugUI;
    #region OnEvent
    public Action OnJoinedOnlineRoomEvent { get; set; }
    public Action OnJoinedOfflineRoomEvent { get; set; }
    #endregion

    private void Awake()
    {
        inc = INetworkConnectGO.GetComponent<INetworkConnect>();
        inc.Init (this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Debug.isDebugBuild)
            DebugUI.SetActive(true);
    }

    #region Network Transmission Token
    public GameObject RequestTokenHandler(SyncTokenType tokenType, Transform parent)
    {
        //TODO: ObjectPooling!
        var go = Instantiate(NetworkSyncHandler, parent);
        go.GetComponent<TokenHandler>().Setup(tokenType, parent);
        return go;
    }

    public GameObject RequestSyncToken(InstantiationData datatoSend, Transform trasn)
    {
        return inc.RequestSyncToken(datatoSend, trasn);
    }
    #endregion

    #region
    public bool IsOfflineRoom()
    {
        return inc.IsOfflineRoom();
    }
    #endregion
}
