﻿using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransmissionBase : MonoBehaviourPunCallbacks, ITransmissionBase, IPooledObject
{
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

        InstantiationData data = null;
        if (photonView.InstantiationData != null)
            data = new InstantiationData(photonView.InstantiationData);
        else
            return;

        Debug.Log($"TransmissionBase Start {data}");
        tType = data.tokenType;
        switch (tType)
        {
            case SyncTokenType.Player:
                var pta = gameObject.AddComponent<PlayerCoreAdditive>();
                pta.Init(this, data);
                break;
            default:
            case SyncTokenType.General:
                var rta = gameObject.AddComponent<RoomCoreAdditive>();
                rta.Init(this, data);
                lcom.Add(rta);

                var osa = gameObject.AddComponent<OwnershipSubAdditive>();
                osa.Init(this, data);
                lcom.Add(osa);

                rta.enabled = true;
                osa.enabled = true;
                break;
        }

        //foreach (var ita in gameObject.GetComponents<ITokenAdditive>())
        //    ita.Init(this. data);
        started = true;
    }

    public override void OnDisable()
    {
        started = false;

        base.OnDisable();

        for (int i =lcom.Count-1;i>=0;i--)
        {
            lcom[i].enabled = false;
        }
        lcom.Clear();
    }

    List<MonoBehaviour> lcom = new List<MonoBehaviour>();

    public bool started = false;

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