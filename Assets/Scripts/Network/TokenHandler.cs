using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour, ITokenHandler
{
    //readonly Dictionary<string, SerializableReadWrite> dic = new Dictionary<string, SerializableReadWrite>();
    [Header("Created On Joined Room")]
    [SerializeField] TransmissionBase transToken;

    [Header("Debug Purpose")]
    [SerializeField]
    [Tooltip("Determine Interat with either Room/Player Properties")]
    SyncTokenType tokenType;
    [SerializeField]
    object refObject;

    ISyncHandlerUser tokenUser;
    ITokenProvider tokenProvider;

    private void OnEnable()
    {
        //Register for Token with NetworkSystem
        ServiceManager.Instance.networkSystem.OnJoinedOnlineRoomEvent += OnJoinedOnlineRoomAct;
    }

    private void OnDisable()
    {
        ServiceManager.Instance.networkSystem.OnJoinedOnlineRoomEvent -= OnJoinedOnlineRoomAct;
    }

    #region Checker
    public bool HavingToken()
    {
        return transToken != null;
    }
    #endregion

    public void Setup(ITokenProvider itp, SyncTokenType tType, object refObj)
    {
        tokenProvider = itp;

        tokenType = tType;
        refObject = refObj;

        tokenUser = (refObject as GameObject).GetComponent<ISyncHandlerUser>();
    }

    public void Register(params SerializableReadWrite[] srws)
    {
        if (!HavingToken())
        {
            Debug.LogWarning($"Not Yet InRoom for Register");
            return;
        }
        transToken.Register(srws);
    }

    public void Unregister(params SerializableReadWrite[] srws)
    {
        if (!HavingToken())
        {
            Debug.LogWarning($"Not Yet InRoom for Unregister");
            return;
        }
        transToken.Unregister(srws);
    }

    #region PlayerProperties: direct set
    public bool PushStateInto(string key, object data)
    {
        if (!HavingToken())
        {
            Debug.Log($"NotInRoom");
            return false;
        }

        // Bsed on registered tokenType, use corresponding helper, Data will sync through Player/RoomProperties 
        transToken.UpdateProperties(tokenType, key, data);
        return true;
    }
    #endregion

    public virtual void OnJoinedOnlineRoomAct()
    {
        Debug.Log($"[TokenHandler] OnJoinedOnlineRoomAct");

        // Online InRoom Create a NetworkedSyncToken
        var datatoSend = InstantiationData.Build(tokenType);
        datatoSend.Add("syncPUNTrans", "true");
        //datatoSend.Add("syncPlayerPos","true");
        //datatoSend.Add("syncPlayerRot", "true");

        GameObject ntGO = tokenProvider.RequestSyncToken(datatoSend, refObject) as GameObject;
        if (ntGO != null)
        {
            transToken = ntGO.GetComponent<TransmissionBase>();
        }
    }
}