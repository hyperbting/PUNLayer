using System;
using UnityEngine;

public class TokenHandler : MonoBehaviour, ITokenHandler
{
    //readonly Dictionary<string, SerializableReadWrite> dic = new Dictionary<string, SerializableReadWrite>();
    [Header("Created On Joined Room")]
    [SerializeField] GameObject trasnTokenGO;

    ISyncHandlerUser tokenUser;
    ITokenProvider tokenProvider;

    public Action<InstantiationData> OnJoinedOnlineRoomEventBeforeTokenCreation { get; set; }
    //public Action<ITransmissionBase> OnJoinedOnlineRoomEventAfterTokenCreation { get; set; }

    #region Checker
    public bool HavingToken()
    {
        return trasnTokenGO != null;
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
        tokenProvider = itp;

        tokenUser = handlerUser;

        ServiceManager.Instance.networkSystem.OnJoinedOnlineRoomEvent += TryOnJoinedRoomAct;

        TryOnJoinedRoomAct();

        this.enabled = true;
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
        trasnTokenGO.GetComponent<TransmissionBase>().UpdateProperties(tokenUser.SupplyInstantiationData.tokenType, key, data);
        return true;
	}
    #endregion

    #region
    public object CreateInRoomObject()
    {
        if (!HavingToken())
        {
            Debug.Log($"NotInRoom");
            return null;
        }

        var datatoSend = InstantiationData.Build(SyncTokenType.General);
        datatoSend.Add("RenameGO", "InRoomObject");
        targetObj = tokenProvider.RequestSyncToken(datatoSend) as GameObject;

        return targetObj;
    }

    public bool DestroyTargetObject()
    {
        if (targetObj == null)
        {
            Debug.Log($"NoTarget");
            return false;
        }

        PrefabPoolManager.Instance.Destroy(targetObj);
        targetObj = null;
        return true;
    }

    public GameObject targetObj;
    public void RequestOwnership()
    {
        if (targetObj == null)
        {
            Debug.Log($"targetObj missing");
            return;
        }
        var scr = targetObj.GetComponent<OwnershipSubAdditive>();
        if (scr == null)
        {
            Debug.Log($"targetObj OwnershipSubAdditive missing");
            return;
        }

        if (trasnTokenGO == null)
        {
            Debug.Log($"trasnTokenGO missing");
            return;
        }

        var tb = trasnTokenGO.GetComponent<TransmissionBase>();
        if (tb == null)
        {
            Debug.Log($"TransmissionBase missing");
            return;
        }

        _ = scr.RequestOwnership(tb.GetOwner());
    }

    public void ReleaseOwnership()
    {
        if (targetObj == null)
        {
            Debug.Log($"targetObj missing");
            return;
        }
        var scr = targetObj.GetComponent<OwnershipSubAdditive>();
        if (scr == null)
        {
            Debug.Log($"targetObj OwnershipSubAdditive missing");
            return;
        }

        _ = scr.ReleaseOwnership();
    }
    #endregion

    void TryOnJoinedRoomAct()
    {
        if (!ServiceManager.Instance.networkSystem.IsOnlineRoom())
        {
            Debug.Log($"[TokenHandler] TryOnJoinedRoomAct NotInOnlineRoom");
            return;
        }

        OnJoinedOnlineRoomAct();
    }

    public virtual void OnJoinedOnlineRoomAct()
    {
        Debug.Log($"[TokenHandler] OnJoinedOnlineRoomAct");

        // Online InRoom Create a NetworkedSyncToken
        InstantiationData datatoSend = tokenUser?.SupplyInstantiationData;

        OnJoinedOnlineRoomEventBeforeTokenCreation?.Invoke(datatoSend);

        InRoomCreateTrasnToken(datatoSend);
    }

    public void InRoomCreateTrasnToken(InstantiationData datatoSend)
    {
        trasnTokenGO = tokenProvider.RequestSyncToken(datatoSend) as GameObject;
        if (!HavingToken())
        {
            Debug.LogWarning($"Not Yet InRoom for Register");
            return;
        }

        if (tokenUser?.SerializableReadWrite != null)
        {
            Debug.LogWarning($"TokenUser Sync: {tokenUser.SerializableReadWrite.Length}");

            var ish = trasnTokenGO.GetComponent<SerializableHelper>();
            ish.Register(tokenUser.SerializableReadWrite);
            ish.enabled = true;
        }
        else
        {
            Debug.LogWarning($"No TokenUser for Sync");
        }
    }

}