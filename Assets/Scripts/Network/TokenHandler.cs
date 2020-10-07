using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour
{
    SyncTokenType tokenType;
    Transform refTransform;

    public TransmissionBase transToken;

    [Header("Debug Purpose")]
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

    public void Setup(SyncTokenType tType, Transform refTran)
    {
        tokenType = tType;
        refTransform = refTran;
    }

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