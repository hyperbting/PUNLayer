using System;
using UnityEngine;

public class TokenHandler : MonoBehaviour, ITokenHandler
{
    //readonly Dictionary<string, SerializableReadWrite> dic = new Dictionary<string, SerializableReadWrite>();
    [Header("Created On Joined Room")]
    [SerializeField] TransmissionBase transToken;

    [Header("Debug Purpose")]
    [Tooltip("Determine Interat with either Room/Player Properties")]
    [SerializeField] SyncTokenType tokenType;
    [SerializeField] object refObject;

    ISyncHandlerUser tokenUser;
    ITokenProvider tokenProvider;

    public Action<InstantiationData> OnJoinedOnlineRoomEventBeforeTokenCreation { get; set; }
    //public Action<ITransmissionBase> OnJoinedOnlineRoomEventAfterTokenCreation { get; set; }

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
        var obj = tokenProvider.RequestSyncToken(datatoSend, gameObject);

        var ntGO = obj as GameObject;
        if (ntGO != null)
        {
            targetObj = ntGO;
            //ntGO.name = "InRoomObject";
            //transToken = ntGO.GetComponent<TransmissionBase>();
        }

        return obj;
    }

    public bool DestroyTargetObject()
    {
        if (!HavingToken())
        {
            Debug.Log($"NotInRoom");
            return false;
        }

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

    GameObject trasnTokenGO;
    public virtual void OnJoinedOnlineRoomAct()
    {
        Debug.Log($"[TokenHandler] OnJoinedOnlineRoomAct");

        // Online InRoom Create a NetworkedSyncToken
        var datatoSend = InstantiationData.Build(tokenType);

        OnJoinedOnlineRoomEventBeforeTokenCreation?.Invoke(datatoSend);

        trasnTokenGO = tokenProvider.RequestSyncToken(datatoSend, refObject) as GameObject;
        if (trasnTokenGO != null)
        {
            transToken = trasnTokenGO.GetComponent<TransmissionBase>();
            //OnJoinedOnlineRoomEventAfterTokenCreation?.Invoke(transToken);
        }
    }
}