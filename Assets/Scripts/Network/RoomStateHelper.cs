using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// Attach this to each RoomObject that needs to be synced at OnJoinedRoom/ OnJoinedOnlineRoom
/// Define UNIQUE stateKey for each 
/// State for Item is implemented by RoomProperties with OnRoomPropertiesUpdate() and PhotonNetwork.CurrentRoom.SetCustomProperties()
public class RoomStateHelper : BaseSyncHelper
{
    [Header("Settings")]
    public string stateKey = "defRP";
    public bool mcJoinedRoomMaintain = true;
    //public Action<string> initJoinRoomLoad;
    
    Action<string> OnRoomStateRegistered;
    Action<string> OnRoomStateUnregistered;
    
    #region stateKeyRepo to reminder which key is used in RoomState
    static List<string> stateKeyRepo;
    List<string> StateKeyRepo
    {
        get
        {
            if(stateKeyRepo == null)
                stateKeyRepo = new List<string>();
            return stateKeyRepo;
        }
    }

    bool TryAddStateKey(string newKey)
    {

        if (string.IsNullOrWhiteSpace(newKey) || stateKey == "defRP")
        {
            Debug.LogError("StateKey Invalid!; RoomStateHelper WON'T work properly");
            return false;
        }
        
        if (StateKeyRepo.Contains(newKey))
        {
            Debug.LogError($"StateKey {newKey} USED somewhere!; RoomStateHelper WON'T work properly");
            return false;
        }
        
        StateKeyRepo.Add(newKey);
        return true;
    }

    bool TryRemoveStateKey(string targetKey)
    {
        StateKeyRepo.Remove(targetKey);
        return true;
    }

    #endregion
    
    public override void OnEnable()
    {
        base.OnEnable();

        OnRoomStateRegistered += OnRoomStateRegisteredAct;
        OnRoomStateUnregistered += OnRoomStateUnregisteredAct;
        
        TryAddStateKey(stateKey);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        
        OnRoomStateRegistered -= OnRoomStateRegisteredAct;
        OnRoomStateUnregistered -= OnRoomStateUnregisteredAct;
        
        TryRemoveStateKey(stateKey);
    }

    public async Task<bool> UpdateRoomProperties(string key)
    {
        if (dataToSync.TryGetValue(key, out SerializableReadWrite srw))
        {
            return await UpdateRoomProperties(key, srw.Read());
        }

        return false;
    }
    
    #region OnRoomStateRegistered/ OnRoomStateUnregistered
    void OnRoomStateRegisteredAct(string keyName)
    {
        if (TryAddStateKey(keyName) && ServiceManager.Instance.networkSystem.IsOnlineRoom())
        {
            // Register and also in OnlineRoom
        }
    }
    
    void OnRoomStateUnregisteredAct(string keyName)
    {
        TryRemoveStateKey(keyName);
    }
    #endregion
    
    #region override ISerializableHelper Register/UnRegister
    public override void Register(SerializableReadWrite srw)
    {
        var newName = $"{stateKey}_{srw.name}";
        Debug.LogError($"RoomStateHelper change name {srw.name} to {newName}");
        srw.name = newName;
        base.Register(srw);

        OnRoomStateRegistered.Invoke(newName);
    }
    
    public override void Unregister(SerializableReadWrite srw)
    {
        var newName = $"{stateKey}_{srw.name}";
        Debug.LogError($"RoomStateHelper change name {srw.name} to {newName}");
        srw.name = newName;
        base.Unregister(srw);
        
        OnRoomStateUnregistered.Invoke(newName);
    }
    #endregion

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

            // Update Info into RoomProperties
            
        }
        else
        {
            Debug.Log($"{stateKey} try to download {roomName}_RoomState");

            // Download RoomProperties to Local
        }
    }

    void OnLeftRoomAct(string roomName)
    {
    }
}
