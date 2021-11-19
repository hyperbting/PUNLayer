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
    public Action<OnOffline> OnJoinedRoomEvent { get; set; }
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
        var go = INetworkConnectGO.GetComponent<ITokenProvider>().RequestSyncToken(datatoSend);
        (go as GameObject).transform.SetParent(transform.root);

        return go;
    }

    //public object RequestManualSyncToken(InstantiationData datatoSend)
    //{
    //    return inc.ManualBuildSyncToken(datatoSend);
    //}

    public void RevokeSyncToken(InstantiationData instData)
    {
        INetworkConnectGO.GetComponent<ITokenProvider>().RevokeSyncToken(instData) ;
    }

    public void RevokeSyncToken(int networkID)
    {
        INetworkConnectGO.GetComponent<ITokenProvider>().RevokeSyncToken(networkID);
    }
    #endregion

    #region InRoom
    public int GetNetworkID()
    {
        return inc.GetNetworkID();
    }
    #endregion

    #region Ownership
    [SerializeField] OwnershipHelper oh;
    public void RequestOwnership(object targetObj)
    {
        oh.RequestOwnership(targetObj);
    }

    public void ReleaseOwnership(object targetObj)
    {
        oh.ReleaseOwnership(targetObj);
    }
    #endregion

    #region RoomObject
    [SerializeField] RoomObjectHelper roh;
    public Transform RoomObjectParent
    {
        get {
            return roh.RoomObjectRoot.transform;
        }
    }

    public void InstantiateRoomObject(InstantiationData insData)
    {
        if (!inc.IsInRoom())
        {
            Debug.LogWarning("[InstantiateRoomObject] Only InRoom can Instantiate RoomObject!");
            return;
        }

        roh.InstantiateRoomObject(insData);
    }

    public void DestroyRoomObject(InstantiationData insData)
    {
        if (!inc.IsInRoom())
        {
            Debug.LogWarning("[InstantiateRoomObject] Only InRoom can Destroy RoomObject!");
            return;
        }

        roh.DestroyRoomObject(insData);
    }
    #endregion

    #region checker?
    public bool IsOnlineRoom()
    {
        return inc.IsInRoom(OnOffline.Online);
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
