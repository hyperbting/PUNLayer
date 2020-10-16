using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransmissionBase : MonoBehaviourPunCallbacks, ITransmissionBase
{
    #region Properties
    [SerializeField] SerializableHelper seriHelper;
    public ISerializableHelper SeriHelper
    {
        get
        {
            if(seriHelper == null)
                seriHelper = GetComponent<SerializableHelper>();
            return seriHelper as ISerializableHelper;
        }
    }

    [SerializeField] StateHelper statHelper;
    public ISerializableHelper StatHelper
    {
        get
        {
            if (statHelper == null)
                statHelper = GetComponent<StateHelper>();
            return statHelper as ISerializableHelper;
        }
    }
    #endregion

    public bool started = false;
    protected virtual void Start()
    {
        var data = new InstantiationData(photonView.InstantiationData);

        Debug.Log($"TransmissionBase Start {data}");
        switch (data.tokenType)
        {
            case SyncTokenType.Player:
                var pta = gameObject.AddComponent<PlayerAdditive>();
                pta.Init(this, data);
                break;
            default:
            case SyncTokenType.General:
                var rta = gameObject.AddComponent<RoomAdditive>();
                rta.Init(this, data);

                var osa = gameObject.AddComponent<OwnershipAdditive>();
                osa.Init(this, data);

                break;
        }

        if (photonView.IsMine)
        {
            Debug.Log($"I Own {photonView.ViewID} {PhotonNetwork.LocalPlayer.UserId} " + photonView.Owner.ToStringFull());
        }
        else
        {
            // deal with photonView.InstantiationData
            Debug.Log($"{photonView.ViewID} TryLoadData for {photonView.Owner.UserId}");
            Debug.Log($"InstantiationDataLength:{photonView.InstantiationData.Length}");
            for(int i = 0; i < photonView.InstantiationData.Length; i++)
                Debug.Log($"InstantiationData: {i} {photonView.InstantiationData[i]}");
        }

        started = true;
    }

    //// for Owner
    //List<SerializableReadWrite> srw = new List<SerializableReadWrite>();
    //public void Setup(List<SerializableReadWrite> srws)
    //{
    //    srw = srws;
    //    Invoke("RegisterSerializableReadWrite", 0);
    //}

    #region Setup SerializableHelper/ StateHelper
    public void Setup(bool useSerialize=false)
    {
        (SeriHelper as SerializableHelper).enabled = useSerialize;
    }
    #endregion

    #region Register
    public void Register(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
        {
            switch (srw.syncType)
            {
                case SyncHelperType.RoomState:
                case SyncHelperType.PlayerState:
                    statHelper.Register(srw);
                    break;
                case SyncHelperType.Serializable:
                    seriHelper.Register(srw);
                    break;
                default:
                    break;
            }
        }
    }

    public void Unregister(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
        {
            switch (srw.syncType)
            {
                case SyncHelperType.RoomState:
                case SyncHelperType.PlayerState:
                    statHelper.Unregister(srw.name);
                    break;
                case SyncHelperType.Serializable:
                    seriHelper.Unregister(srw.name);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region Use SerializableHelper/ StateHelper
    public void UpdateProperties(SyncTokenType stType, string key, object data)
    {
        switch (stType)
        {
            case SyncTokenType.Player:
                statHelper.UpdatePlayerProperties(key, data);
                break;
            default:
            case SyncTokenType.General:
                _ = statHelper.UpdateRoomProperties(key, data);
                break;
        }
    }
    #endregion
}
