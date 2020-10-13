using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour
{
    Dictionary<string, SerializableReadWrite> dic = new Dictionary<string, SerializableReadWrite>();
    [Header("Created On Joined Room")]
    [SerializeField] TransmissionBase transToken;

    [Header("Debug Purpose")]
    [SerializeField]
    [Tooltip("Determine Interat with either Room/Player Properties")]
    SyncTokenType tokenType;
    [SerializeField]
    Transform refTransform;

    ISyncTokenUser tokenUser;

    private void OnEnable()
    {
        //Register for Token with NetworkSystem
        ServiceManager.Instance.networkSystem.OnJoinedOnlineRoomEvent += OnJoinedOnlineRoomAct;
    }

    private void OnDisable()
    {
        ServiceManager.Instance.networkSystem.OnJoinedOnlineRoomEvent -= OnJoinedOnlineRoomAct;
    }

    #region
    public bool HavingToken()
    {
        return transToken != null;
    }
    #endregion

    public void Setup(SyncTokenType tType, Transform refTran)
    {
        tokenType = tType;
        refTransform = refTran;

        tokenUser = refTran.GetComponent<ISyncTokenUser>();
    }

    //public void Register(ISyncTokenUser tokenUser)
    //{
    //    this.tokenUser = tokenUser;
    //}

    #region PlayerProperties: direct set
    public bool PushStateInto(string key, object data)
    {
        if (transToken == null)
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

        GameObject ntGO = ServiceManager.Instance.networkSystem.RequestSyncToken(datatoSend, refTransform);
        if (ntGO != null)
            transToken = ntGO.GetComponent<TransmissionBase>();

        tokenUser.RegisterWithTransmissionToken(transToken);

        //switch (tokenType)
        //{
        //    case SyncTokenType.Player:
        //        break;
        //    default:
        //        break;
        //}
    }
}