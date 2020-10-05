using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

using System.Threading;
using System.Threading.Tasks;

public class Player : MonoBehaviour
{
    public PlayerTransmission transmissionToken;
    Transform transmissionTransform;
    [Space]

    public ItemHolder itHolder;
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

        if (itHolder == null)
            itHolder = GetComponent<ItemHolder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //
        pInput.Player.Fire.performed += Fire;
        //pInput.Player.FireSub.performed += FireSub;

        //Request TokenHandler From NetworkManager
        var th = ServiceManager.Instance.networkSystem.RequestTokenHandler(this.transform);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHost || pInput == null)
            return;

        around = pInput.Player.Look.ReadValue<Vector2>();
        move = pInput.Player.Move.ReadValue<Vector2>();
        // Update orientation first, then move. Otherwise move orientation will lag
        // behind by one frame.
        Look(around);
        Move(move);

        if (pInput.Player.LookMouseEnable.ReadValue<float>() >= 1)
        {
            Look(pInput.Player.LookMouse.ReadValue<Vector2>());
        }

        UpdateTokenTransform();
    }

    private void Fire(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        //CreatePersonalItem
        itHolder.CreateLocalItemBase();
    }

    private void FireSub(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        //if (!Photon.Pun.PhotonNetwork.InRoom || ctx.ReadValue<float>() < 0.5)
        //    return;

        //Photon.Pun.PhotonNetwork.Instantiate("PUNNetowrkObject", transform.position, Quaternion.identity);
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

    private void UpdateTokenTransform()
    {
        if (transmissionToken != null && transmissionTransform != null)
        {
            transmissionTransform.position = transform.position;
            transmissionTransform.rotation = transform.rotation;
        }
    }

    #region SerilizableReadWrite
    public void RegisterWithTransmissionToken(PlayerTransmission pt)
    {
        Debug.Log("RegisterWithTransmissionToken");
        transmissionToken = pt;
        transmissionTransform = pt.transform;
        Debug.Log("RegisterWithTransmissionToken BuildSerlizableData");
        foreach (var kv in itHolder.BuildSerlizableData())
        {
            //kv.Value
        }

        //var listToSerializableSync = new List<SerializableReadWrite >()
        //{
        //    new SerializableReadWrite ("Random", ReadValue, WriteValue),
        //    //new SerilizableReadWrite(ReadRotation, WriteRotation) { name = "SyncRot" }
        //};
        //pt.Setup(listToSerializableSync);
    }

    //object ReadValue()
    //{
    //    return Time.time;
    //}

    //void WriteValue(object obj)
    //{
    //    Debug.Log($"{(float)obj}");
    //}
    #endregion SerilizableReadWrite
}
