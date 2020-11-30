﻿using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class RaiseEventHelper: MonoBehaviour, IOnEventCallback, IRaiseEventHelper
{
    public static RaiseEventHelper instance;

    static readonly byte MyOwnRaiseEventCode = 198;
    public Dictionary<string, Action<object[]>> dic = new Dictionary<string, Action<object[]>>();

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

    [Header("Debug")]
    [SerializeField] ReceiverGroup receivers = ReceiverGroup.All;
    [SerializeField] EventCaching cachingOption = EventCaching.AddToRoomCache;
    [SerializeField] string evContnt;
    Hashtable ht = new Hashtable();
    public bool RaiseEvent(object[] evContnet)
    {
        Debug.Log($"[RaiseEventHelper] RaiseEvent {evContnet[0].ToString()} @{Time.time}");

        ht.CleanThenInsert(evContnet);

        if (evContnt.Length > 0)
        {
            ht.ReplaceFirstElement(evContnt);
            Debug.Log($"[RaiseEventHelper] evContnt Modified {ht.ToString()} @{Time.time}");
        }

        RaiseEventOptions evOption = new RaiseEventOptions()
        {
            Receivers = receivers,
            CachingOption = cachingOption
        };

        return PhotonNetwork.RaiseEvent( MyOwnRaiseEventCode, ht, evOption, SendOptions.SendReliable);
    }

    public bool RemoveCachedEvent(object[] evContnet)
    {
        Debug.Log($"[RaiseEventHelper] RemoveCachedEvent @{Time.time}");
        cachingOption = EventCaching.RemoveFromRoomCache;
        return RaiseEvent(evContnet);
    }

    #region IOnEventCallback
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode != MyOwnRaiseEventCode)
            return;

        Debug.Log($"[RaiseEventHelper] OnEvent {photonEvent.ToStringFull()}");
        var ht = (Hashtable)photonEvent.CustomData;
        var objs = ht.FormObjects();

        if (dic.TryGetValue(objs[0].ToString(), out Action<object[]> dealerObj))
        {
            dealerObj(objs);
        }
    }
    #endregion
}

public static class PhotonHashtableExtensions
{
    public static void CleanThenInsert(this ExitGames.Client.Photon.Hashtable ht, object[] objs)
    {
        ht.Clear();

        for (byte i = 0; i < objs.Length; i++)
            ht[i] = objs[i];
    }

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