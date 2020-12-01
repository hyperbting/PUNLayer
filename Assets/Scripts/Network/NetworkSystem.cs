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

    #region INetworkConnectUser
    public Action OnJoinedOnlineRoomEvent { get; set; }
    public Action OnJoinedOfflineRoomEvent { get; set; }
    #endregion

    #region ITokenProvider. Network Transmission Token
    public ITokenHandler RequestTokenHandlerAttachment(SyncTokenType tokenType, object refScript)
    {
        var go = (refScript as Component).gameObject;
        if (go == null)
        {
            Debug.LogWarning($"RequestTokenHandler {refScript} is not Attach2GameObject");
            return null;
        }

        var res = go.AddComponent<TokenHandler>();
        res.Setup(this, tokenType, go);
        return res;
    }

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

    #region checker?
    public bool IsOfflineRoom()
    {
        return inc.IsOfflineRoom();
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
