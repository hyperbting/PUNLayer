using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class BaseSyncHelper : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Tmp Hashtable for WritePreparation
    /// </summary>
    protected ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
    protected ExitGames.Client.Photon.Hashtable orght = new ExitGames.Client.Photon.Hashtable();

    /// <summary>
    /// To Store method to read/ write specific value
    /// </summary>
    protected Dictionary<string, SerializableWrite> dataToSync = new Dictionary<string, SerializableWrite>();
    #region Registration
    public void Register(SerializableWrite srw)
    {
        if (!dataToSync.ContainsKey(srw.name))
        {
            dataToSync.Add(srw.name, srw);
            Debug.Log($"{srw.name} Registered");
        }
    }

    public void Register(params SerializableWrite[] srws)
    {
        foreach (var srw in srws)
            Register(srw);
    }

    public void Unregister(SerializableWrite srw)
    {
        Unregister(srw.name);
    }

    public void Unregister(params SerializableWrite[] srws)
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

    #region direct touch
    public void UpdateProperties(string key, object value, SyncTokenType stt)
    {
        ht.Clear();
        ht.Add(key, value);

        switch (stt)
        {
            case SyncTokenType.Player:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
                break;
            default:
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
                break;
        }
    }
    #endregion
}
