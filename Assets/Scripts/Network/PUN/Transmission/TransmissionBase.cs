using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransmissionBase : MonoBehaviourPunCallbacks, ITransmissionBase, IPooledObject
{
    [SerializeField] OwnershipSubAdditive osa;
    ICoreAdditive playerCoreAdditive;
    ICoreAdditive roomCoreAdditive;

    [SerializeField] SyncTokenType tType;
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
    public override void OnEnable()
    {
        base.OnEnable();

        var targets = GetComponents<ICoreAdditive>();
        foreach (var tar in targets)
        {
            switch (tar.AdditiveType)
            {
                case SyncTokenType.Player:
                    playerCoreAdditive = tar;
                    break;
                case SyncTokenType.General:
                    roomCoreAdditive = tar;
                    break;
                default:
                    Debug.LogWarning($"Unprepared Detected {tar.AdditiveType}");
                    break;
            }
        }

        if (photonView.InstantiationData == null)
            return;

        InstantiationData data = new InstantiationData(photonView.InstantiationData);
        Debug.Log($"TransmissionBase Start. photonView.IsMine:{photonView.IsMine} {data}");
        Init(data);
        started = true;
    }

    public override void OnDisable()
    {
        started = false;

        base.OnDisable();
    }

    public bool started = false;

    //// for Owner
    //List<SerializableReadWrite> srw = new List<SerializableReadWrite>();
    //public void Setup(List<SerializableReadWrite> srws)
    //{
    //    srw = srws;
    //    Invoke("RegisterSerializableReadWrite", 0);
    //}

    public void Init(InstantiationData data)
    {        
        tType = data.tokenType;
        switch (tType)
        {
            case SyncTokenType.Player:
                playerCoreAdditive.Init(data, photonView.IsMine);

                break;
            default:
            case SyncTokenType.General:
                if (photonView.IsMine)
                    roomCoreAdditive.Init(data, photonView.IsMine);

                osa.Init(this, data);

                break;
        }
    }

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

    public Photon.Realtime.Player GetOwner()
    {
        return photonView.Owner;
    }

    #region IPooledObject
    [SerializeField] string assignedName;
    [SerializeField] PrefabPool parentPool;
    public PrefabPool GetParentPool { get {
            if (parentPool == null)
                parentPool = transform.parent.GetComponent<PrefabPool>();

            return parentPool;
        }
    }

    public void Init(object[] data)
    {
        assignedName = (string)data[0];
        parentPool = (PrefabPool)data[1];
    }

    public void Reset()
    {
        gameObject.name = assignedName;
    }
    #endregion
}