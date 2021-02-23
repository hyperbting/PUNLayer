using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransmissionBase : MonoBehaviourPunCallbacks, ITransmissionBase, IPooledObject
{
    ICoreAdditive playerCoreAdditive;
    ICoreAdditive roomCoreAdditive;

    [SerializeField] GameObject refObj;
    public object RefObject
    {
        get
        {
            return refObj;
        }
        set
        {
            refObj = value as GameObject;
        }
    }

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

        InstantiationData data = new InstantiationData(photonView.InstantiationData);

        Debug.Log($"TransmissionBase Start REMOTEly.");
        Setup(data);
    }

    public override void OnDisable()
    {
        started = false;

        base.OnDisable();
    }

    /// <summary>
    /// Setup by 
    /// TokenHandler LOCALly 
    /// or 
    /// using data received from TransmissionBase.OnEnable
    /// </summary>
    /// <param name="insData"></param>
    public void Setup(InstantiationData insData)
    {
        tType = insData.tokenType;
        Debug.Log($"TransmissionBase {gameObject.name} Setup. photonView.IsMine:{photonView.IsMine} {insData}");

        // get tokenUser from ObjectSupplier
        switch (tType)
        {
            case SyncTokenType.Player:
                playerCoreAdditive.Init(insData, photonView.IsMine);

                break;
            default:
            case SyncTokenType.General:
                Debug.LogWarning("TransmissionBase Setup General");
                roomCoreAdditive.Init(insData, photonView.IsMine);
                break;
        }

        // Setup SerializableReadWrite
        var sData = refObj?.GetComponent<ISyncHandlerUser>()?.SerializableReadWrite;
        if (sData != null)
        {
            Register(sData);
        }
        else
        {
            Debug.LogWarning($"TransmissionBase {gameObject.name}: No TokenUser/ no SerializableReadWrite for Sync!");
        }

        started = true;
    }

    #region Register
    public void Register(params SerializableReadWrite[] srws)
    {
        Debug.LogWarning($"TransmissionBase {gameObject.name} Register:{srws.Length} SerializableReadWrite");

        bool stateOn = false;
        bool serialOn = false;
        foreach (var srw in srws)
        {
            switch (srw.syncType)
            {
                case SyncHelperType.RoomState:
                case SyncHelperType.PlayerState:
                    statHelper.Register(srw);
                    stateOn = true;
                    break;
                case SyncHelperType.Serializable:
                    seriHelper.Register(srw);
                    serialOn = true;
                    break;
                default:
                    break;
            }
        }

        statHelper.enabled = stateOn;
        seriHelper.enabled = serialOn;
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