using System;
using System.Linq;
using UnityEngine;

public partial class Player : MonoBehaviour, ISyncHandlerUser
{
    ITokenHandler tokHandler;

    [Space]
    public bool isHost = false;

    Action OnEnableEvent;
    Action OnDisableEvent;
    Action OnAwakeEvent;
    Action OnFixedUpdateEvent;

    public void OnEnable()
    {
        pInput.Enable();
        OnEnableEvent?.Invoke();
    }

    public void OnDisable()
    {
        pInput.Disable();
        OnDisableEvent?.Invoke();
    }

    public void Awake()
    {
        DoAwake();
        OnAwakeEvent?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isHost)
        {
            return;
        }
        SetupInputSystem();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoFixedUpdate();
        OnFixedUpdateEvent?.Invoke();
    }

    #region ISyncHandlerUser; talk to TokenHandler; Called By SyncToken when OnJoinedOnlineRoom
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

    //For remote, Setupby ITransmissionBase
    public void Init(InstantiationData data, bool isMine)
    {
        Debug.LogWarning($"ISyncHandlerUser Init isMine?{isMine}");
        insdata = data;

        //// Local HostPlayer Ask for a TokenHandler
        if (isMine)
        {
            SetupTokenHandler();
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