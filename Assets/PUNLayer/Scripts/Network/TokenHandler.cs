﻿using System;
using UnityEngine;

/// <summary>
/// Token handler is used By ISyncHandlerUser to Setup Transmission Automatically
/// </summary>
public class TokenHandler : MonoBehaviour, ITokenHandler
{
    //readonly Dictionary<string, SerializableReadWrite> dic = new Dictionary<string, SerializableReadWrite>();
    [Header("Created On Joined Room")]
    [SerializeField] GameObject trasnTokenGO;

    ISyncHandlerUser tokenUser;
    ITokenProvider tokenProvider;

    public Action<InstantiationData> OnJoinedOnlineRoomEventBeforeTokenCreation { get; set; }
    //public Action<ITransmissionBase> OnJoinedOnlineRoomEventAfterTokenCreation { get; set; }

    private void OnDestroy()
    {
        ServiceManager.Instance.networkSystem.OnJoinedRoomEvent -= TryOnJoinedRoomAct;

        if (trasnTokenGO == null)
            return;

        var networkID = trasnTokenGO.GetComponent<IOwnershipInteractable>().GetNetworkID();
        if(networkID >0)
            tokenProvider.RevokeSyncToken(networkID);
    }

    #region Checker
    public bool HavingToken()
    {
        return trasnTokenGO != null;
	}
    
    public bool HavingMineToken()
    {
        if (!HavingToken())
            return false;

        var tb = trasnTokenGO.GetComponent<ITransmissionBase>();
        if (tb == null)
            return false;

        return tb.IsMine();
    }
    #endregion

    #region Getter
    public object GetGameObject()
    {
        return gameObject as object;
    }
    #endregion

    #region Setter
    [SerializeField] byte[] instrestedGroupID = new byte[] { 0 };
    [SerializeField] byte[] unInstrestedGroupID = new byte[] {};
    public void SetInterestGroup()
    {
        //Debug.Log($"SetInterestGroup {groupID}");
        //transToken.photonView.Group = groupID;
        Photon.Pun.PhotonNetwork.SetInterestGroups(unInstrestedGroupID, instrestedGroupID);
    }
    #endregion

    public void Setup(ITokenProvider itp, ISyncHandlerUser handlerUser)
    {
        Debug.Log($"TokenHandler Setup");

        tokenProvider = itp;
        tokenUser = handlerUser;
        ServiceManager.Instance.networkSystem.OnJoinedRoomEvent += TryOnJoinedRoomAct;
        
        if(ServiceManager.Instance.networkSystem.IsOnlineRoom())
            TryOnJoinedRoomAct(OnOffline.Online);

        this.enabled = true;
    }

    #region JoinedRoom
    void TryOnJoinedRoomAct(OnOffline ooline)
    {
        if(HavingMineToken())
        {
            Debug.LogWarning($"TryOnJoinedRoomAct Found My TransmissionToken!");
            Destroy(trasnTokenGO);
        }
        
        if (ooline != OnOffline.Online)
        {
            Debug.Log($"[TokenHandler] TryOnJoinedRoomAct NotInOnlineRoom");
            return;
        }

        OnJoinedOnlineRoomAct();
    }

    public virtual void OnJoinedOnlineRoomAct()
    {
        if (HavingToken())
        {
            Debug.LogWarning($"[TokenHandler] OnJoinedOnlineRoomAct Having Token");
            return;
        }

        Debug.Log($"[TokenHandler] OnJoinedOnlineRoomAct");

        //// Online InRoom Load InstaData from TokenUser
        InstantiationData datatoSend = tokenUser?.SupplyInstantiationData;

        OnJoinedOnlineRoomEventBeforeTokenCreation?.Invoke(datatoSend);

        //// InRoom RequestSyncToken
        trasnTokenGO = tokenProvider.RequestSyncToken(datatoSend) as GameObject;
        if (!HavingToken())
        {
            Debug.LogWarning($"Not Yet InRoom for Register");
            return;
        }

        trasnTokenGO.GetComponent<ITransmissionBase>()?.Setup(datatoSend);
    }
    #endregion

 //   #region PlayerProperties: direct set
 //   public bool PushStateInto(string key, object data)
 //   {
 //       if (!HavingToken())
 //       {
 //           Debug.Log($"NotInRoom");
 //           return false;
 //       }

 //       // Bsed on registered tokenType, use corresponding helper, Data will sync through Player/RoomProperties 
 //       trasnTokenGO.GetComponent<TransmissionBase>().UpdateProperties(tokenUser.SupplyInstantiationData.tokenType, key, data);
 //       return true;
	//}
 //   #endregion

 //   #region
 //   public object CreateInRoomObject()
 //   {
 //       if (!HavingToken())
 //       {
 //           Debug.Log($"NotInRoom");
 //           return null;
 //       }

 //       var datatoSend = InstantiationData.Build(SyncTokenType.General);
 //       datatoSend.Add("RenameGO", "InRoomObject");

 //       return tokenProvider.RequestSyncToken(datatoSend);
 //   }

 //   public bool DestroyTargetObject(object targetObj)
 //   {
 //       if (targetObj == null)
 //       {
 //           Debug.Log($"NoTarget");
 //           return false;
 //       }

 //       //PrefabPoolManager.Instance.Destroy(targetObj);
 //       return true;
 //   }
 //   #endregion
}