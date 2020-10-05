using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class BaseSyncHelper : MonoBehaviourPunCallbacks
{
    protected ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();

    protected Dictionary<string, SerializableReadWrite > dataToSync = new Dictionary<string, SerializableReadWrite >();

    #region Registration
    public void Register(SerializableReadWrite srw)
    {
        if (!dataToSync.ContainsKey(srw.name))
        {
            dataToSync.Add(srw.name, srw);
            Debug.Log($"{srw.name} Registered");
        }
    }

    public void Register(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
            Register(srw);
    }

    public void Unregister(SerializableReadWrite srw)
    {
        Unregister(srw.name);
    }

    public void Unregister(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
            Unregister(srw.name);
    }

    public void Unregister(string key)
    {
        if (!dataToSync.ContainsKey(key))
            return;

        Debug.Log($"{key} Unregistered");
        dataToSync.Remove(key);
    }
    #endregion   
}
