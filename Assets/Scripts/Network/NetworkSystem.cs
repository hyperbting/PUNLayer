using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSystem : MonoBehaviour, INetworkConnectUser, ITokenProvider
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
            DebugUI?.SetActive(true);
    }

    #region ITokenProvider. Network Transmission Token
    public object RequestTokenHandler(SyncTokenType tokenType, object refObj)
    {
        //TODO: ObjectPooling!
        var go = Instantiate(NetworkSyncHandler, (refObj as GameObject).transform);
        go.GetComponent<ITokenHandler>().Setup(this, tokenType, refObj);
        return go;
    }

    public object RequestSyncToken(InstantiationData datatoSend, object refObj)
    {
        Transform refTran = null;
        if (refObj != null)
            refTran = (refObj as GameObject).transform;
        return inc.RequestSyncToken(datatoSend, refTran) as object;
    }

    public object RequestManualSyncToken(InstantiationData datatoSend)
    {
        return inc.ManualBuildSyncToken(datatoSend);
    }
    #endregion

    #region
    public bool IsOfflineRoom()
    {
        return inc.IsOfflineRoom();
    }
    #endregion
}
