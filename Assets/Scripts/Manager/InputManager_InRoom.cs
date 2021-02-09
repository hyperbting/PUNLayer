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
    [SerializeField] RoomObjectHelper roHelper;
    void CreateRoomObject(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Debug.Log("CreateRoomObject");
        if (ctx.ReadValue<float>() < 0.5)
            return;

        var data = new InstantiationData() { tokenType = SyncTokenType.General };
        data[InstantiationData.InstantiationKey.objectname.ToString()] = roObjectPrefab.name;

        roHelper?.InstantiateroomObject(data);
    }
}
