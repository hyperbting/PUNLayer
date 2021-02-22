using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InputManager : MonoBehaviour
{
    void InRoomSetup()
    {
        Debug.LogWarning($"InRoomSetup");

        pInput.InRoom.CreateHostPlayer.performed += CreateHostPlayer;
        pInput.InRoom.RemoveHostPlayer.performed += RemoveHostPlayer;

        pInput.InRoom.CreateRoomObject.performed += CreateRoomObject;
        //pInput.InRoom.RemoveRoomObject.performed += ;

        pInput.InRoom.TryInteract.performed += TryClickSelect;
    }

    [Header("HostPlayer")]
    [SerializeField] PlayerMaker playerMaker;
    void CreateHostPlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Debug.Log("CreateHostPlayer");
        if (ctx.ReadValue<float>() < 0.5)
            return;

        if (playerMaker.GetMine() != null)
        {
            Debug.Log("HostPlayer Created");
            return;
        }

        playerMaker.InstantiateObject();
    }

    void RemoveHostPlayer(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Debug.Log("RemoveHostPlayer");
        if (ctx.ReadValue<float>() < 0.5)
            return;

        if (!playerMaker.GetMine())
        {
            Debug.Log("HostPlayer Null");
            return;
        }

        playerMaker.DestroyObject();
    }

    [Header("RoomObject")]
    [SerializeField] GameObject roObjectPrefab;
    void CreateRoomObject(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        Debug.Log("TryCreateRoomObject");
        var data = new InstantiationData() { tokenType = SyncTokenType.General };
        data[InstantiationData.InstantiationKey.objectname.ToString()] = roObjectPrefab.name;
        data[InstantiationData.InstantiationKey.sceneobject.ToString()] = "create";

        ServiceManager.Instance.networkSystem.InstantiateRoomObject(data);
    }

    void DestroyRoomObject(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() < 0.5)
            return;

        Debug.Log("TryDestroyRoomObject");

        if (ServiceManager.Instance.interactionManager.IsMine())
        {
            var tknUser = (ServiceManager.Instance.interactionManager.TargetObject as GameObject).GetComponent<ISyncHandlerUser>();
            var instData = tknUser.SupplyInstantiationData;
            instData[InstantiationData.InstantiationKey.sceneobject.ToString()] = "destroy";
            ServiceManager.Instance.networkSystem.DestroyRoomObject(instData);
        }

        Debug.LogWarning($"Cannot Destroy NonOwnedRoomObject");
    }

    void TryClickSelect(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
#if ENABLE_INPUT_SYSTEM
        Vector3 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();//pInput.Player.MousePosition.ReadValue<Vector2>();
#else
        Vector3 mousePosition = Input.mousePosition;
#endif
        //Debug.Log("TryClickSelect");
        ServiceManager.Instance.interactionManager.MousePointer(mousePosition);
    }
}
