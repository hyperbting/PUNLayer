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
        pInput.Player.Devour.performed += SwitchTransformSyncType;
    }

    private void SwitchTransformSyncType(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        switch (rps.CurrentType)
        {
            case TransformSyncType.None:
                SwitchTransformSyncType(TransformSyncType.PhotonViewTransform);
                break;
            case TransformSyncType.PhotonViewTransform:
                SwitchTransformSyncType(TransformSyncType.SerializeView);
                break;
            case TransformSyncType.SerializeView:

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
