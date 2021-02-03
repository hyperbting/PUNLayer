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

    public bool started = false;
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

        if (photonView.IsMine)
            return;

        InstantiationData data = new InstantiationData(photonView.InstantiationData);
        Debug.Log($"TransmissionBase Start Remotely.");
        Setup(data);

        started = true;
    }

    public override void OnDisable()
    {
        started = false;

        base.OnDisable();
    }

    public void Setup(InstantiationData insData)
    {
        tType = insData.tokenType;
        Debug.Log($"TransmissionBase {gameObject.name} Setup. photonView.IsMine:{photonView.IsMine} {insData}");

        ISyncHandlerUser tokenUser = null;
        switch (tType)
        {
            case SyncTokenType.Player:
                tokenUser = playerCoreAdditive.Init(insData, photonView.IsMine);
                break;
            default:
            case SyncTokenType.General:
                tokenUser = roomCoreAdditive.Init(insData, photonView.IsMine);

                osa.Init(insData);

                break;
        }

        // Setup SerializableReadWrite
        if (tokenUser?.SerializableReadWrite != null)
        {
            //Debug.LogWarning($"TransmissionBase tokenUser.SerializableReadWrite: {tokenUser.SerializableReadWrite.Length}");
            Register(tokenUser.SerializableReadWrite);
        }
        else
        {
            Debug.LogWarning($"TransmissionBase {gameObject.name}: No TokenUser/ no SerializableReadWrite for Sync!");
        }
    }

    #region Register
    public void Register(params SerializableReadWrite[] srws)
    {
        Debug.LogWarning($"TransmissionBase {gameObject.name} Register:{srws.Length} SerializableReadWrite");
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