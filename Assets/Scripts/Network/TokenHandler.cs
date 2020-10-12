using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour
{
    Dictionary<string, SerializableReadWrite> dic = new Dictionary<string, SerializableReadWrite>();

    [Header("Debug Purpose")]
    [SerializeField]
    [Tooltip("Determine Interat with either Room/Player Properties")]
    SyncTokenType tokenType;
    [SerializeField]
    Transform refTransform;
    [Space]
    [SerializeField] TransmissionBase transToken;
    [SerializeField] Transform transmissionTransform;

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
    }

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

    #region Register
    public void Register(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
        {
            // Already In Room
            if (HavingToken())
            {
                transToken.Register(srws);
            }

            dic[srw.name] = srw;
        }
    }

    public void Unregister(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
            if (dic.ContainsKey(srw.name))
            {
                dic.Remove(srw.name);

                // Already In Room
                if (HavingToken())
                {
                    transToken.Unregister(srw);
                }
            }
    }
    #endregion

    public virtual void OnJoinedOnlineRoomAct()
    {
        Debug.Log($"[TokenHandler] OnJoinedOnlineRoomAct");

        // Online InRoom Create a NetworkedSyncToken
        GameObject ntGO = ServiceManager.Instance.networkSystem.RequestSyncToken(tokenType, refTransform);
        if (ntGO != null)
            transToken = ntGO.GetComponent<TransmissionBase>();

        switch (tokenType)
        {
            case SyncTokenType.Player:
                var hostPlayerGO = PlayerManager.Instance.GetHostPlayer();
                hostPlayerGO.GetComponent<ISyncTokenUser>().RegisterWithTransmissionToken(transToken);
                break;
            default:
                break;
        }
    }
}