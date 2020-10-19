using UnityEngine;

public class Player : MonoBehaviour, ISyncHandlerUser
{
    ITokenHandler tokHandler;

    [Space]

    [Space]
    public bool isHost = false;

    public float moveSpeed;
    public float rotateSpeed;
    public PUN2Tester pInput;

    [Header("Debug")]
    [SerializeField] Vector2 move;
    [SerializeField] Vector2 around;

    public void OnEnable()
    {
        pInput.Enable();
    }

    public void OnDisable()
    {
        pInput.Disable();
    }

    public void Awake()
    {
        pInput = new PUN2Tester();

        //if (itHolder == null)
        //    itHolder = GetComponent<ItemHolder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isHost)
        {
            return;
        }
        SetupSync();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHost || pInput == null)
            return;

        around = pInput.Player.Look.ReadValue<Vector2>();
        move = pInput.Player.Move.ReadValue<Vector2>();

        // Update orientation first, then move. Otherwise move orientation will lag behind by one frame.
        Look(around);
        Move(move);

        if (pInput.Player.LookMouseEnable.ReadValue<float>() >= 1)
        {
            Look(pInput.Player.LookMouse.ReadValue<Vector2>());
        }

        //UpdateTokenTransform();
    }

    #region InputSystem Actions
    private void Fire(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01)
            return;
        var scaledMoveSpeed = moveSpeed * Time.deltaTime;
        // For simplicity's sake, we just keep movement in a single plane here. Rotate direction according to world Y rotation of player.
        var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        transform.position += move * scaledMoveSpeed;
    }

    private void Look(Vector2 rotate)
    {
        if (rotate.sqrMagnitude < 0.01)
            return;
        var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
        var m_Rotation = transform.localEulerAngles;
        m_Rotation.y += rotate.x * scaledRotateSpeed;
        //m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);
        transform.localEulerAngles = m_Rotation;
    }

    private void Echo(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        if (tokHandler == null || !tokHandler.HavingToken())
            return;

        tokHandler.PushStateInto("k1", Time.fixedTime);
    }
    #endregion

    #region ISyncHandlerUser; talk to TokenHandler; Called By SyncToken when OnJoinedOnlineRoom
    void SetupSync()
    {
        if (!isHost)
        {
            return;
        }

        //// isHost()
        pInput.Player.Fire.performed += Fire;
        pInput.Player.Echo.performed += Echo;

        //Request TokenHandler From NetworkManager
        tokHandler = (ServiceManager.Instance.networkSystem.RequestTokenHandler(SyncTokenType.Player, gameObject) as GameObject)
            .GetComponent<ITokenHandler>();

        //What Ability this obj will have
        tokHandler.OnJoinedOnlineRoomEventBeforeTokenCreation += (initdata) => {
            initdata.Add("syncPUNTrans", "true");
            initdata.Add("ablePlayerEcho", "true");
        };

        //tokHandler.OnJoinedOnlineRoomEventAfterTokenCreation += (trans) => { };
    }

    public void SetupSync(ITransmissionBase itb, InstantiationData data)
    {
        //Debug.Log("SetupSync");
        if (data.TryGetValue("ablePlayerEcho", out string val) && val == "true")
        {
            itb.Register(BuildEchoSerializableReadWrite());
        }
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
    #endregion SerilizableReadWrite


}