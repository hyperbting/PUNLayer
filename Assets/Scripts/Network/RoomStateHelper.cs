using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// State for Item is implemented by RoomProperties with OnRoomPropertiesUpdate() and PhotonNetwork.CurrentRoom.SetCustomProperties()
public class RoomStateHelper : BaseSyncHelper
{
    [Header("Settings")]
    public string stateKey = "defRP";
    public bool mcJoinedRoomMaintain = true;
    //public Action<string> initJoinRoomLoad;
    
    public override void OnEnable()
    {
        base.OnEnable();

        if (string.IsNullOrWhiteSpace(stateKey) || stateKey == "defRP")
        {
            Debug.LogError("StateKey Invalid");
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public async Task<bool> UpdateRoomProperties(string key)
    {
        if (dataToSync.TryGetValue(key, out SerializableReadWrite srw))
        {
            return await UpdateRoomProperties(key, srw.Read());
        }

        return false;
    }
    
    #region override ISerializableHelper Register/UnRegister
    public override void Register(SerializableReadWrite srw)
    {
        Debug.LogError($"RoomStateHelper {stateKey} Register");
        base.Register(srw);
    }
    
    public override void Unregister(SerializableReadWrite srw)
    {
        Debug.LogError($"RoomStateHelper{stateKey} Unregister");
        base.Unregister(srw);
    }
    #endregion
    
    [Header("Debug Info")]
    [SerializeField]
    private List<KeyObjectPair> kvPair;

    void OnJoinedRoomAct(string roomName)
    {
        if (ServiceManager.Instance.networkSystem.inc.IsRoomOwner())
        {
            if (mcJoinedRoomMaintain)
            {
                Debug.Log($"{stateKey} try to maintain {roomName} Room State");

                // if (initJoinRoomLoad != null)
                // {
                //     Debug.Log($"{stateKey} try to download Room State");
                //     initJoinRoomLoad.Invoke(roomName);   
                // }
            }

            //// Update Info into RoomProperties
            //_ = UpdateRoomProperties();
        }
        else
        {
            Debug.Log($"{stateKey} try to download {roomName} Room State");
            
        }
    }

    void OnLeftRoomAct(string roomName)
    {
    }
}
