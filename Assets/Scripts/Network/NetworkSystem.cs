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
    public GameObject INetworkSyncHandler;

    INetworkConnect inc;

    [Space]
    [Header("Debug")]
    public GameObject DebugUI;
    #region OnEvent
    public Action OnJoinedRoomEvent { get; set; }
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

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Network Transmission Token
    //TODO: ObjectPooling!
    public GameObject RequestTokenHandler(Transform parent)
    {
        return Instantiate(INetworkSyncHandler, parent);
    }

    //static Transform tokenParent;
    //public Transform BuildTokenParent()
    //{
    //    if (tokenParent != null)
    //        return tokenParent;

    //    var go = GameObject.Find("TokenParent");
    //    if (go != null)
    //        tokenParent = go.transform;
    //    else
    //        tokenParent = new GameObject("TokenParent").transform;

    //    return tokenParent;
    //}

    public GameObject RequestSyncToken(Transform trasn)
    {
        var go = Photon.Pun.PhotonNetwork.Instantiate("PlayerTransmissionToken", trasn.position, trasn.rotation);
        //go.transform.SetParent(BuildTokenParent());

        return go;
    }
    #endregion
}
