using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class BaseSyncHelper : MonoBehaviourPunCallbacks, ISerializableHelper
{
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

    #region direct touch
    public void UpdatePlayerProperties(string key, object value)
    {
        ht.Clear();
        ht.Add(key, value);

        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }

    //Check And Swap For Properties
    TaskCompletionSource<bool> roomPropUpdateResult;
    public static string updateKey;
    object updateVal;
    public async Task<bool> UpdateRoomProperties(string key, object value)
    {
        if (updateKey != null)
        {
            Debug.Log("Was UpdateRoomProperties");
            return false;
        }

        updateKey = key;

        ht.Clear();
        ht.Add(key, value);

        orght.Clear();
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out object rVal))
        {
            orght.Add(key, rVal);
        }
        else
        {
            orght.Add(key, null);
        }

        if (!PhotonNetwork.CurrentRoom.SetCustomProperties(ht, orght))
        {
            updateKey = null;
            return false;
        }

        updateVal = value;
        roomPropUpdateResult = new TaskCompletionSource<bool>();

        return await roomPropUpdateResult.Task;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        foreach (var de in propertiesThatChanged)
        {
            if (updateKey == de.Key.ToString() && updateVal == de.Value)
            {
                updateKey = null;
                updateVal = null;

                roomPropUpdateResult?.TrySetResult(true);
            }
        }
    }

    // CAS failure
    private void NetworkingClientOnOpResponseReceived(OperationResponse opResponse)
    {
        if (opResponse.OperationCode == OperationCode.SetProperties && 
            opResponse.ReturnCode == ErrorCode.InvalidOperation &&
            updateKey != null &&
            opResponse.DebugMessage != null &&
            opResponse.DebugMessage.Contains(updateKey))
        {
            updateKey = null;
            updateVal = null;
            roomPropUpdateResult?.TrySetResult(false);
        }
    }
    #endregion
}
