using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class RaiseEventHelper: MonoBehaviour, IOnEventCallback, NetworkLayer.IRoomEventHelper
{
    readonly string scr = "RaiseEventHelper";
    public static RaiseEventHelper instance;

    static readonly byte MyOwnRaiseEventCode = 198;
    Dictionary<string, NetworkLayer.RoomEventRegistration> dic = new Dictionary<string, NetworkLayer.RoomEventRegistration>();

    #region IRaiseEventHelper
    public void Register(NetworkLayer.RoomEventRegistration rer)
    {
        dic[rer.key] = rer;
        Debug.Log($"{scr} {rer.key} registered");
    }

    public void Unregister(string key)
    {
        if (dic.ContainsKey(key))
        {
            dic.Remove(key);
            Debug.Log($"{scr} {key} unregistered");
        }
    }
    #endregion

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Awake()
    {
        instance = this;
    }

    //[Header("Debug")]
    //[SerializeField] ReceiverGroup receivers = ReceiverGroup.All;
    //[SerializeField] EventCaching cachingOption = EventCaching.AddToRoomCache;
    //[SerializeField] string evContnt;
    //Hashtable ht = new Hashtable();

    public bool RaiseEvent(string key, object[] data)
    {
        var extendedByteArray2 = new object[data.Length + 1];
        extendedByteArray2[0] = key;
        Array.Copy(data, 0, extendedByteArray2, 1, data.Length);

        return RaiseEvent(extendedByteArray2);
    }

    public bool RaiseEvent(object[] evContnet)
    {
        Debug.Log($"[RaiseEventHelper] RaiseEvent {evContnet[0]} {evContnet[1]} @{Time.time}");

        if (dic.TryGetValue((string)evContnet[0], out NetworkLayer.RoomEventRegistration dealerObj))
        {
            RaiseEventOptions evOption = new RaiseEventOptions()
            {
                Receivers = (Photon.Realtime.ReceiverGroup)dealerObj.receivers,
                CachingOption = (Photon.Realtime.EventCaching)dealerObj.cachingOption
            };

            return PhotonNetwork.RaiseEvent(
                MyOwnRaiseEventCode, 
                evContnet, 
                evOption, 
                SendOptions.SendReliable
                );
        }

        Debug.Log($"[RaiseEventHelper] Cannot found {evContnet[0]}");
        return false;
    }

    public bool RemoveCachedEvent(object[] evContnet)
    {
        Debug.Log($"[RaiseEventHelper] RemoveCachedEvent @{Time.time}");

        if (dic.TryGetValue((string)evContnet[0], out NetworkLayer.RoomEventRegistration dealerObj))
        {
            RaiseEventOptions evOption = new RaiseEventOptions()
            {
                Receivers = (Photon.Realtime.ReceiverGroup)dealerObj.receivers,
                CachingOption = EventCaching.RemoveFromRoomCache
            };

            return PhotonNetwork.RaiseEvent(
                MyOwnRaiseEventCode,
                evContnet,
                evOption,
                SendOptions.SendReliable
                );
        }

        return false;
    }

    #region IOnEventCallback
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode != MyOwnRaiseEventCode)
            return;

        object[] objs = (object[])photonEvent.CustomData;
        if (objs == null)
        {
            Debug.LogWarning($"[RaiseEventHelper] OnEvent NullData");
            return;
        }

        if (dic.TryGetValue((string)objs[0], out NetworkLayer.RoomEventRegistration dealerObj))
        {
            dealerObj.OnRoomEvent?.Invoke(objs);
        }
    }
    #endregion
}

public static class PhotonHashtableExtensions
{
    // public static void CleanThenInsert(this ExitGames.Client.Photon.Hashtable ht, object[] objs)
    // {
    //     ht.Clear();
    //
    //     for (byte i = 0; i < objs.Length; i++)
    //         ht[i] = objs[i];
    // }

    public static object[] FormObjects(this ExitGames.Client.Photon.Hashtable ht)
    {
        var data = new object[ht.Count];
        for (byte i = 0; i < data.Length; i++)
            data[i] = ht[i];

        return data;
    }

    public static void ReplaceFirstElement(this ExitGames.Client.Photon.Hashtable ht, object obj)
    {
        ht[(byte)1] = obj;
    }
}