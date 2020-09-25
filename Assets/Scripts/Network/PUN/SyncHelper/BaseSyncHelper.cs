using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class BaseSyncHelper : MonoBehaviourPunCallbacks
{
    protected ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();

    protected Dictionary<string, SerilizableReadWrite> dataToSync = new Dictionary<string, SerilizableReadWrite>();

    #region interface
    public void Register(SerilizableReadWrite srw)
    {
        if (!dataToSync.ContainsKey(srw.name))
        {
            dataToSync.Add(srw.name, srw);
            Debug.Log($"{srw.name} Registered");
        }
    }

    public void Unregister(SerilizableReadWrite srw)
    {
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
