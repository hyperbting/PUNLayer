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
        ServiceManager.Instance.networkSystem.OnJoinedRoomEvent += OnJoinedRoomAct;
    }

    private void OnDisable()
    {
        ServiceManager.Instance.networkSystem.OnJoinedRoomEvent -= OnJoinedRoomAct;
    }

    public void Setup(SyncTokenType tType, Transform refTran)
    {
        tokenType = tType;
        refTransform = refTran;
    }

    public virtual void OnJoinedRoomAct()
    {
        Debug.Log($"[TokenHandler] OnJoinedRoomAct");
        GameObject ntGO = ServiceManager.Instance.networkSystem.RequestSyncToken(tokenType, refTransform);
        if (ntGO != null)
            transToken = ntGO.GetComponent<TransmissionBase>();

        switch (tokenType)
        {
            case SyncTokenType.Player:
                var hostPlayerGO = PlayerManager.Instance.GetHostPlayer();
                hostPlayerGO.GetComponent<Player>().RegisterWithTransmissionToken(transToken as PlayerTransmission);
                break;
            default:
                break;
        }
    }
}