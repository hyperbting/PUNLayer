using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

using System.Threading;
using System.Threading.Tasks;

public class Player : MonoBehaviour
{
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
    }

    // Start is called before the first frame update
    void Start()
    {
        pInput.Player.Fire.performed += Fire;
        pInput.Player.FireSub.performed += FireSub;

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
    }

    private void Fire(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!Photon.Pun.PhotonNetwork.InRoom || ctx.ReadValue<float>() < 0.5)
            return;

        var go = Photon.Pun.PhotonNetwork.InstantiateRoomObject("PUNNetowrkObject", transform.position, Quaternion.identity);
        var pv = go.GetComponent<Photon.Pun.PhotonView>();
        pv.TransferOwnership(Photon.Pun.PhotonNetwork.LocalPlayer);
        _ = ReleaseOwnerShip(pv);
    }

    async Task ReleaseOwnerShip(Photon.Pun.PhotonView pv)
    {
        await Task.Delay(1);
        pv.TransferOwnership(null);
    }

    private void FireSub(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!Photon.Pun.PhotonNetwork.InRoom || ctx.ReadValue<float>() < 0.5)
            return;

        Photon.Pun.PhotonNetwork.Instantiate("PUNNetowrkObject", transform.position, Quaternion.identity);
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
}
