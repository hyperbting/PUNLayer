using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseSyncHelper : MonoBehaviourPunCallbacks, ISerializableHelper
{
    protected KeyObjectPair[] CurrentRoomProperties
    {
        get
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.CustomProperties == null)
                return null;

            return PhotonNetwork.CurrentRoom.CustomProperties.ToKeyObjectPair();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.OpResponseReceived += NetworkingClientOnOpResponseReceived;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.OpResponseReceived -= NetworkingClientOnOpResponseReceived;
    }

    /// <summary>
    /// Tmp Hashtable for WritePreparation
    /// </summary>
    protected ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
    protected ExitGames.Client.Photon.Hashtable orght = new ExitGames.Client.Photon.Hashtable();

    /// <summary>
    /// To Store method to read/ write specific value
    /// </summary>
    protected Dictionary<string, SerializableReadWrite> dataToSync = new Dictionary<string, SerializableReadWrite>();
    #region Registration
    public virtual void Register(SerializableReadWrite srw)
    {
        if (!dataToSync.ContainsKey(srw.name))
        {
            dataToSync.Add(srw.name, srw);
            Debug.Log($"{srw.name} Registered ({dataToSync.Keys.Count}");
        }
    }

    public void Register(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
            Register(srw);
    }

    public virtual void Unregister(SerializableReadWrite srw)
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

    #region direct touch
    public void UpdatePlayerProperties(string key, object value)
    {
        ht.Clear();
        ht.Add(key, value);

        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }

    //Check And Swap For Properties
    TaskCompletionSource<bool> roomPropUpdateResult;

    string BuildSerialNumber(string keyBase)
    {
        return $"{keyBase}-{Random.Range(0, 1000).ToString()}";
            
    }

    public static string updateRPKey="rpSerial";
    public static string updateRPValue;
    [SerializeField] private ExitGames.Client.Photon.Hashtable tmpHT;
    public async Task<bool> UpdateRoomProperties(string key, object value)
    {
        if (updateRPValue != null)
        {
            Debug.Log("Was UpdateRoomProperties");
            return false;
        }

        updateRPValue = BuildSerialNumber("rp");

        ht.Clear();
        ht.Add(key, value);
        ht.Add(updateRPKey, updateRPValue);

        tmpHT = PhotonNetwork.CurrentRoom.CustomProperties;
        
        orght.Clear();
        if (tmpHT.TryGetValue(key, out object rVal))
        {
            orght.Add(key, rVal);
        }
        else
        {
            orght.Add(key, null);
        }

        if (!PhotonNetwork.CurrentRoom.SetCustomProperties(ht, orght))
        {
            updateRPValue = null;
            return false;
        }
        
        roomPropUpdateResult = new TaskCompletionSource<bool>();

        return await roomPropUpdateResult.Task;
    }
    
    public async Task<bool> UpdateRoomProperties(params KeyObjectPair[] kvpair)
    {
        if (updateRPValue != null)
        {
            Debug.Log("Was UpdateRoomProperties");
            return false;
        }

        ht.CleanInsert(kvpair);
        updateRPValue = BuildSerialNumber("rp");
        ht[updateRPKey] = updateRPValue;
        
        tmpHT = PhotonNetwork.CurrentRoom.CustomProperties;

        orght.CleanInsertDefaultNull(tmpHT,kvpair);

        if (!PhotonNetwork.CurrentRoom.SetCustomProperties(ht, orght))
        {
            updateRPValue = null;
            return false;
        }
        
        roomPropUpdateResult = new TaskCompletionSource<bool>();

        return await roomPropUpdateResult.Task;
    }
    
    #endregion
    
    #region PhotonCallbacks
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // One End for the UpdateRoomProperties; another possible end is NetworkingClientOnOpResponseReceived
        if (propertiesThatChanged.TryGetValue(updateRPKey, out object value) && value != null && updateRPValue == (string)value)
        {
            updateRPValue = null;
            roomPropUpdateResult?.TrySetResult(true);
        }
        
        //Who should NOT react to this ?
        List<string> keys = propertiesThatChanged.StringKeysIntersect(dataToSync);
        if (keys.Count() <= 0)
        {
            return;
        }

        // apply every Registered Room States to local
        foreach (var key in keys)
        {
            if (dataToSync.TryGetValue(key, out SerializableReadWrite srw))
            {
                srw?.Write(propertiesThatChanged[key]);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        // only apply to local for PhotonView Owner
        if (targetPlayer != photonView.Owner)
            return;
        
        // apply every Registered Player States to local
        foreach (var key in changedProps.Keys)
        {
            if (dataToSync.TryGetValue(key.ToString(), out SerializableReadWrite srw))
            {
                srw.Write(changedProps[key]);
            }
        }
    }
    
    // CAS failure
    // One End for the UpdateRoomProperties; another possible end is OnRoomPropertiesUpdate
    private void NetworkingClientOnOpResponseReceived(OperationResponse opResponse)
    {
        if (opResponse.OperationCode == OperationCode.SetProperties && 
            opResponse.ReturnCode == ErrorCode.InvalidOperation &&
            updateRPValue != null &&
            opResponse.DebugMessage != null &&
            opResponse.DebugMessage.Contains(updateRPValue))
        {
            updateRPValue = null;
            roomPropUpdateResult?.TrySetResult(false);
        }
    }
    #endregion
}

public static class PhotonHashtableExtensions2
{
    public static void CleanInsert(this ExitGames.Client.Photon.Hashtable ht, params KeyObjectPair[] kvPair)
    {
        ht.Clear();
        for (int i = 0; i < kvPair.Length; i++)
        {
            ht.Add(kvPair[i].k, kvPair[i].v);   
        }
    }
    
    public static void CleanInsertDefaultNull(this ExitGames.Client.Photon.Hashtable ht, ExitGames.Client.Photon.Hashtable refHT, params KeyObjectPair[] kvPair)
    {
        ht.Clear();
        for (int i = 0; i < kvPair.Length; i++)
        {
            if(refHT.ContainsKey(kvPair[i].k))
                ht.Add(kvPair[i].k, kvPair[i].v);
            else
            {
                ht.Add(kvPair[i].k, null);
            }
        }
    }
    
    public static List<string> StringKeys(this ExitGames.Client.Photon.Hashtable ht)
    {
        List<string> result = new List<string>();
        foreach (object ke in ht.Keys)
        {
            if(ke != null)
                result.Add((string)ke);
        }

        return result;
    }

    public static List<string> StringKeysIntersect<T>(this ExitGames.Client.Photon.Hashtable ht, Dictionary<string,T> stringObjectDic)
    {
        return ht.StringKeys().Intersect(stringObjectDic.Keys.ToList()).ToList();//intersection two set of key
    }

    public static Dictionary<string, object> ToDictionaryStringObject(this ExitGames.Client.Photon.Hashtable ht)
    {
        var result = new Dictionary<string, object>();
        foreach (var kvp in ht)
        {
            result[(string)kvp.Key] = kvp.Value;
        }

        return result;
    }
    public static KeyObjectPair[] ToKeyObjectPair(this ExitGames.Client.Photon.Hashtable ht)
    {
        var result = new KeyObjectPair[ht.Count];
        int idx = 0;
        foreach (var kvp in ht)
        {
            result[idx] = new KeyObjectPair()
            {
                k=(string)kvp.Key,
                v = kvp.Value
            };
            idx++;
        }

        return result;
    }
    
}

[Serializable]
public class KeyObjectPair
{
    public string k;
    public object v;
}