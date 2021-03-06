﻿using System;
using System.Linq;
using UnityEngine;

public partial class Player : MonoBehaviour, ISyncHandlerUser
{
    public PlayerMaker creator;

    ITokenHandler tokHandler;

    [Space]
    public bool isHost = false;

    Action OnEnableEvent;
    Action OnDisableEvent;
    Action OnAwakeEvent;
    Action OnFixedUpdateEvent;

    public void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    public void OnDisable()
    {
        OnDisableEvent?.Invoke();
    }

    public void Awake()
    {
        OnAwakeEvent?.Invoke();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OnFixedUpdateEvent?.Invoke();
    }

    #region ISyncHandlerUser; talk to TokenHandler; Called By SyncToken when OnJoinedOnlineRoom
    public object GameObject()
    {
        return gameObject;
    }

    [SerializeField] InstantiationData insdata;
    public InstantiationData SupplyInstantiationData
    {
        get
        {
            return insdata;
        }
    }

    [SerializeField] PlayerAbility playerAbility;
    public SerializableReadWrite[] SerializableReadWrite
    {
        get
        {
            var local = new SerializableReadWrite[] {
                //new SerializableReadWrite("UnitName", GetUnitName, SetUnitName),
                new SerializableReadWrite("Pos", ReadPos, WritePos),
                new SerializableReadWrite("Rot", ReadRot, WriteRot),
            };

            var result = local.Union(playerAbility.SerializableReadWrite).ToArray();
            //Debug.LogWarning($"Player SerializableReadWrite {result.Length}");

            return result;
        }
    }

    public void Init(InstantiationData data, bool isMine, ITransmissionBase tb = null)
    {
        Debug.LogWarning($"ISyncHandlerUser Init isMine?{isMine}");
        insdata = data;

        var pa = GetComponent<PersistenceHelper>();
        string uuid=null;
        if (data.TryGetValue(InstantiationData.InstantiationKey.objectuuid, out object val))
        {
            uuid = (string)val;
        }

        //// Local HostPlayer Ask for a TokenHandler
        if (isMine)
        {
            SetupTokenHandler();
            pa.Init(uuid);
        }
        else
        {
            pa.Init(
                uuid, 
                ()=> { creator.RemoveFromDict(uuid); }, 
                tb as TransmissionBase
            );

            if (data.TryGetValue(InstantiationData.InstantiationKey.objectpersist, out val))
            {
                pa.Setup((int)val);
            }
        }

        //RaiseEventHelper.instance.Register(new NetworkLayer.RoomEventRegistration()
        //{
        //    key = "Emit",
        //    OnRoomEvent = EmitPropToLocal,
        //    cachingOption = NetworkLayer.EventCaching.DoNotCache,
        //    receivers = NetworkLayer.EventTarget.All
        //});
    }

    //// For Local, TokenUser MUST know WHERE to get TokenHandler
    void SetupTokenHandler()
    {
        Debug.Log($"ISyncHandlerUser SetupTokenHandler");
        tokHandler = ServiceManager.Instance.networkSystem.RequestTokenHandlerAttachment(insdata.tokenType, this);

        //var thGO = ServiceManager.Instance.networkSystem.RequestTokenHandler(insdata.tokenType, this) as GameObject;
        //thGO.transform.SetParent(transform);
    }

    SerializableReadWrite BuildEchoSerializableReadWrite()
    {
        return new SerializableReadWrite(
            "k1",
            null,
            EchoPropToLocal
            ){ syncType = SyncHelperType.PlayerState };
    }

    //object EchoLocalToProp()
    //{
    //    return Time.fixedTime;
    //}

    void EchoPropToLocal(object obj)
    {
        Debug.Log($"{gameObject.name} k1 {obj}"); 
    }

    void EmitPropToLocal(object[] objs)
    {
        Debug.Log($"Emit EmitPropToLocal:");
        foreach(var obj in objs)
            Debug.Log($"{obj}");
    }

    public Transform refTransform;
    void WritePos(object pos)
    {
        //Debug.Log($"WritePos {pos}");
        refTransform.position = (Vector3)pos;
    }

    object ReadPos()
    {
        //Debug.Log($"ReadPos {refTransform.position}");
        return refTransform.position;
    }

    void WriteRot(object rot)
    {
        refTransform.rotation = (Quaternion)rot;
    }

    object ReadRot()
    {
        return refTransform.rotation;
    }
    #endregion SerilizableReadWrite
}