using UnityEngine;

public class Player : MonoBehaviour, ISyncTokenUser
{
    public PlayerTransmission transmissionToken;
    Transform transmissionTransform;
    [Space]

    //public ItemHolder itHolder;
    [Space]
    public bool isHost = false;

    public float moveSpeed;
    public float rotateSpeed;
    public PUN2Tester pInput;

    [Header("Debug")]
    [SerializeField]
    Vector2 move;
    [SerializeField]
    Vector2 around;

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
            return;

        //
        pInput.Player.Fire.performed += Fire;
        pInput.Player.Echo.performed += Echo;

        //Request TokenHandler From NetworkManager
        var th = ServiceManager.Instance.networkSystem.RequestTokenHandler(SyncTokenType.Player, this.transform);
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

        UpdateTokenTransform();
    }

    #region InputSystem Actions
    private void Fire(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        ////CreatePersonalItem
        //itHolder.CreateLocalItemBase();
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

        if (transmissionToken != null)
        {
            transmissionToken.StatHelper.UpdateProperties("k1", Time.fixedTime, SyncTokenType.Player);
        }
    }
    #endregion

    private void UpdateTokenTransform()
    {
        if (transmissionToken != null && transmissionTransform != null)
        {
            transmissionTransform.position = transform.position;
            transmissionTransform.rotation = transform.rotation;
        }
    }

    #region ISyncTokenUser; SerilizableReadWrite talk to TokenHandler; Called By SyncToken when OnJoinedOnlineRoom
    public void RegisterWithTransmissionToken(ITransmissionBase pt)
    {
        Debug.Log("RegisterWithTransmissionToken");
        transmissionToken = pt as PlayerTransmission;
        transmissionTransform = transmissionToken.transform;

        Debug.Log($"RegisterWithTransmissionToken BuildSerlizableData");
        var data = new SerializableWrite("k1", (object obj) => { Debug.Log($"{gameObject.name} {obj}"); });
        transmissionToken.StatHelper.Register(data);
    }
    #endregion SerilizableReadWrite
}