﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [Header("Player Controller")]
    public float moveSpeed;
    public float rotateSpeed;
    public PUN2Tester pInput;

    [Header("Debug")]
    [SerializeField] Vector2 move;
    [SerializeField] Vector2 around;

    #region mono
    public void DoAwake()
    {
        pInput = new PUN2Tester();
    }

    // Update is called once per frame
    void DoFixedUpdate()
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
            Look(pInput.Player.MousePositionDelta.ReadValue<Vector2>());
        }

        //UpdateTokenTransform();
    }
    #endregion

    #region InputSystem Actions
    void SetupInputSystem()
    {
        if (!isHost)
            return;

        Debug.LogWarning($"SetupInputSystem For Local");

        //// isHost()
        pInput.Player.Fire.performed += Fire;
        pInput.Player.Fire.performed += DebugClick;

        pInput.Player.Echo.performed += Echo;
        pInput.Player.Emit.performed += Emit;
        pInput.Player.Devour.performed += Devour;

        pInput.Player.RequestOwnership.performed += RequestOwner;
        pInput.Player.ReleaseOwnership.performed += ReleaseOwner;

        pInput.Player.LoadScene.performed += LoadScene;

        pInput.Player.ChangeGroup.performed += SetInterestGroup;
    }

    private void DebugClick(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
#if ENABLE_INPUT_SYSTEM
        Vector3 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();//pInput.Player.MousePosition.ReadValue<Vector2>();
#else
        Vector3 mousePosition = Input.mousePosition;
#endif

        //mouseCursor.position = mousePosition;

        var ray = Camera.main.ScreenPointToRay(mousePosition);
        var targets = Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePosition), Mathf.Infinity, LayerMask.GetMask("NetworkView"));
        foreach (var ta in targets)
        {
            //Debug.Log($"{ta.transform.name}");
            var pv = Photon.Pun.PhotonView.Get(ta.collider);
            if (pv != null && Photon.Pun.UtilityScripts.PointedAtGameObjectInfo.Instance != null)
                Photon.Pun.UtilityScripts.PointedAtGameObjectInfo.Instance.SetFocus(pv);
        }
    }

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
        var obj = tokHandler.CreateInRoomObject();
    }

    private void Emit(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        if (tokHandler == null || !tokHandler.HavingToken())
            return;

        if (!RaiseEventHelper.instance.RaiseEvent(new object[] { "Emit", Time.time }))
        {
            Debug.LogWarning($"RaiseEvent Report Error!");
        }
    }

    private void Devour(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        if (tokHandler == null || !tokHandler.HavingToken())
            return;

        Debug.Log($"tokHandler.DestroyTarget");
        tokHandler.DestroyTargetObject();
    }

    private void RequestOwner(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        if (tokHandler == null || !tokHandler.HavingToken())
            return;
        Debug.Log($"tokHandler.RequestOwnership");
        tokHandler.RequestOwnership();
    }

    private void ReleaseOwner(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        if (tokHandler == null || !tokHandler.HavingToken())
            return;
        Debug.Log($"tokHandler.ReleaseOwner");
        tokHandler.ReleaseOwnership();
    }

    public int sceneID = 0;
    private void LoadScene(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        Debug.Log($"Load Scene {sceneID}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void SetInterestGroup(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        Debug.Log($"Set InterestGroup");
        (tokHandler as TokenHandler).SetInterestGroup();
    }
    #endregion

}