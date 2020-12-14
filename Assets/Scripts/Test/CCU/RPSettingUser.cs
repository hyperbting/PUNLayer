using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class RPSettingUser : MonoBehaviour
{
    public PUN2Tester pInput;
    public RPSetting rps;

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

    private void Start()
    {
        pInput.Player.Emit.performed += InstantiateNPC;
        pInput.Player.Devour.performed += DestroyNPC;

        pInput.Player.Echo.performed += SwitchTransformSyncType;

        pInput.Player.Brust.performed += EnableBrust;
        pInput.Player.SwitchBrustAmount.performed += SetupBurstAmount;
    }

    #region Burst
    private void EnableBrust(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        rps.OrderBurst();
    }
    private void SetupBurstAmount(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        rps.SetupBurstAmount();
    }
    #endregion

    private void InstantiateNPC(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {

        if (ctx.ReadValue<float>() < 0.5)
            return;

        rps.InstantiateNPC();
    }

    private void DestroyNPC(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {

        if (ctx.ReadValue<float>() < 0.5)
            return;

        rps.DestroyNPC();
    }

    private void SwitchTransformSyncType(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        switch (rps.CurrentType)
        {
            case TransformSyncType.None:
                SwitchTransformSyncType(TransformSyncType.SerializeViewCurrent);
                break;
            case TransformSyncType.SerializeViewCurrent:

                SwitchTransformSyncType(TransformSyncType.SerializeViewTargetOnly);
                break;
            case TransformSyncType.SerializeViewTargetOnly:

                SwitchTransformSyncType(TransformSyncType.None);
                break;
        }
    }

    void SwitchTransformSyncType(TransformSyncType ty)
    {
        rps.SyncTransformSyncType(ty);

        rps.CurrentType = ty;
    }
}
