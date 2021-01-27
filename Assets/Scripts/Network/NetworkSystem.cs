using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSystem : MonoBehaviour, INetworkConnectUser, ITokenProvider, ITokenHandlerProvider
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

    #region INetworkConnectUser
    public Action OnJoinedOnlineRoomEvent { get; set; }
    public Action OnJoinedOfflineRoomEvent { get; set; }
    #endregion

    #region ITokenHandlerProvider
    public ITokenHandler RequestTokenHandlerAttachment(SyncTokenType tokenType, object refScript)
    {
        Debug.LogWarning($"RequestTokenHandler Try AttachToGameObject");
        var go = (refScript as Component).gameObject;
        if (go == null)
        {
            Debug.LogWarning($"RequestTokenHandler {refScript} is NOT AttachToGameObject");
            return null;
        }

        var handUser = go.GetComponent<ISyncHandlerUser>();

        var res = go.AddComponent<TokenHandler>();
        res.Setup(this, handUser);
        return res;
    }

    public object RequestTokenHandler(SyncTokenType tokenType, object refObj)
    {
        var go = Instantiate(NetworkSyncHandler, (refObj as GameObject).transform);
        var handUser = go.GetComponent<ISyncHandlerUser>();

        go.GetComponent<ITokenHandler>()?.Setup(this, handUser);

        return go;
    }
    #endregion

    #region ITokenProvider. Network Transmission Token
    public object RequestSyncToken(InstantiationData datatoSend)
    {
        return inc.RequestSyncToken(datatoSend, transform.root) as object;
    }

    public object RequestManualSyncToken(InstantiationData datatoSend)
    {
        return inc.ManualBuildSyncToken(datatoSend);
    }
    #endregion

    #region checker?
    public bool IsOfflineRoom()
    {
        return inc.IsOfflineRoom();
    }

    public bool IsOnlineRoom()
    {
        return inc.IsOnlineRoom();
    }
    #endregion

    #region Mono
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
    #endregion
}
