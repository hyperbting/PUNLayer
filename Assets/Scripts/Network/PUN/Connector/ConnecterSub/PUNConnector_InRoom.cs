using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class PUNConnecter : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject transmissionTokenPrefab;

    #region Progress Record
    Dictionary<string, KeyValResultPair> rpInProgress = new Dictionary<string, KeyValResultPair>();
    Dictionary<PlayerKey, KeyValResultPair> ppInProgress = new Dictionary<PlayerKey, KeyValResultPair>();
    #endregion

    #region Room Function
    #region Getter
    public object GetRoomProperty(string key)
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out object val))
        {
            return val;
        }

        return null;
    }

    public object GetPlayerProperty(Player player, string key)
    {
        if (PhotonNetwork.InRoom && player.CustomProperties.TryGetValue(key, out object val))
        {
            return val;
        }

        return null;
    }

    public GameObject RequestSyncToken(InstantiationData dataToSend, Transform trasn)
    {
        //dataToSend.Add("uuid",PhotonNetwork.LocalPlayer.UserId);

        GameObject go = null;
        if (dataToSend.tokenType == SyncTokenType.Player)
            go = PhotonNetwork.Instantiate("TransmissionToken", trasn.position, trasn.rotation, 0, dataToSend.ToData());
        else
        {
            if (PhotonNetwork.IsMasterClient)
                go = PhotonNetwork.InstantiateRoomObject("TransmissionToken", trasn.position, trasn.rotation, 0, dataToSend.ToData());
            else
            {
                Debug.LogWarning($"Non MC Cannot InstantiateRoomObject");
            }
        }

        if (go == null)
            Debug.LogWarning($"Issuing Null GameObject");

        return go;
    }

    // create obj first, attach PhotonView?, assign id
    public bool ManualAttachPhotonView(GameObject go, InstantiationData dataToSend)
    {
        var pView = go.AddComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.AllocateViewID(pView))
        {
            dataToSend.Add("viewID", photonView.ViewID.ToString());
            return true;
        }

        Debug.LogError("Failed to allocate a ViewId.");
        return false;
    }

    public GameObject ManualBuildSyncToken(InstantiationData dataToSend)
    {
        var go = Instantiate(transmissionTokenPrefab);
        var pView = go.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(photonView))
        {
            var raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            var sendOptions = new SendOptions { Reliability = true };

            dataToSend.Add("viewID", photonView.ViewID.ToString());

            PhotonNetwork.RaiseEvent(
                (byte)RaiseEvnetCode.CustomManualInstantiationEventCode, 
                dataToSend.ToData(), 
                raiseEventOptions, 
                sendOptions);

            return go;
        }

        Debug.LogError("Failed to allocate a ViewId.");
        Destroy(go);
        return null;
    }

    public void ManualBuildSyncTokenOnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if ((RaiseEvnetCode)eventCode == RaiseEvnetCode.CustomManualInstantiationEventCode)
        {
            var instData = new InstantiationData((object[])photonEvent.CustomData);
            var tok = Instantiate(transmissionTokenPrefab);

            //setup based on InstantiationData

            PhotonView photonView = tok.GetComponent<PhotonView>();
            if (instData.TryGetValue("viewID", out string vid))
                photonView.ViewID = int.Parse(vid);
            else
                Debug.LogWarning($"ViewID Missing!");
        }
    }
    #endregion

    #region IOnEventCallback
    public void OnEvent(EventData photonEvent)
    {
        OnEventAction?.Invoke(photonEvent);
    }
    #endregion

    ExitGames.Client.Photon.Hashtable toChange = new ExitGames.Client.Photon.Hashtable();
    ExitGames.Client.Photon.Hashtable exptected = new ExitGames.Client.Photon.Hashtable();
    public async Task<bool> SetRoomProperty(KeyValExpPair kvePair)
    {
        while (rpInProgress.ContainsKey(kvePair.key))
        {
            Debug.Log($"SetRoomProperties {kvePair.key} InProgress");
            await Task.Delay(1500);
        }

        Debug.Log($"SetRoomProperties {kvePair.ToString()}");

        //TrySet Room Properties
        toChange.Clear();
        exptected.Clear();
        kvePair.InsertInto(ref toChange, ref exptected);

        var kvrp = new KeyValResultPair(kvePair.key, kvePair.value, kvePair.exp);
        rpInProgress.Add(kvePair.key, kvrp);

        if (PhotonNetwork.CurrentRoom.SetCustomProperties(toChange, exptected))
        {
            //Debug.Log($"{scriptName} SetCustomProperties Waiting");
            await Task.WhenAny(Task.Delay(10000), kvrp.setPropResult.Task);
            if (!kvrp.setPropResult.Task.IsCompleted)
                kvrp.setPropResult?.TrySetResult(false);
        }
        else
        {
            Debug.LogWarning($"{scriptName} SetCustomProperties Immedidate fail");
            kvrp.setPropResult?.TrySetResult(false);
        }

        var result = kvrp.setPropResult.Task.Result;
        Debug.Log($"{result}] {kvrp.ToString()}");

        rpInProgress.Remove(kvePair.key);

        return result;
    }

    public async Task<bool> SetPlayerProperty(Player player, KeyValExpPair kvePair)
    {
        var pk = new PlayerKey(player, kvePair.key);
        while (ppInProgress.ContainsKey(pk))
        {
            Debug.Log($"SetPlayerProperty {pk.ToString()} InProgress");
            await Task.Delay(1500);
        }

        Debug.Log($"SetPlayerProperty {kvePair.ToString()} for " + player.ToStringFull());

        //TrySet Room Properties
        toChange.Clear();
        exptected.Clear();
        kvePair.InsertInto(ref toChange, ref exptected);

        var kvrp = new KeyValResultPair(kvePair.key, kvePair.value, kvePair.exp);
        ppInProgress.Add(pk, kvrp);

        if (player.SetCustomProperties(toChange, exptected))
        {
            await Task.WhenAny(Task.Delay(10000), kvrp.setPropResult.Task);
            if (!kvrp.setPropResult.Task.IsCompleted)
                kvrp.setPropResult?.TrySetResult(false);
        }
        else
        {
            Debug.LogWarning($"{scriptName} SetPlayerProperty Immedidate fail");
            kvrp.setPropResult?.TrySetResult(false);
        }

        //
        var result = kvrp.setPropResult.Task.Result;
        Debug.Log($"{result}] {kvrp.ToString()}");

        ppInProgress.Remove(pk);

        return result;
    }
    #endregion

    #region IInRoomCallbacks 
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        OnPlayerEnteredRoomAction?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        OnPlayerLeftRoomAction?.Invoke(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        foreach (var kvPair in changedProps)
        {
            OnPlayerPropertyUpdateAction?.Invoke(targetPlayer, kvPair.Key.ToString(), kvPair.Value);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        foreach (var kvPair in propertiesThatChanged)
        {
            OnRoomPropertyUpdateAction?.Invoke(kvPair.Key.ToString(), kvPair.Value);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
    }
    #endregion

    #region
    void CompareWithPPInProgress(Player player, string key, object val)
    {
        var pk = new PlayerKey(player, key);
        Debug.Log(pk.ToString());
        if (ppInProgress.TryGetValue(pk, out KeyValResultPair desiredKVRPair) &&
            ((desiredKVRPair.value == null && val == null) || val.Equals(desiredKVRPair.value)))
        {
            // successfully set to RoomProperties
            Debug.Log(pk.ToString() + " ok");
            desiredKVRPair.setPropResult?.TrySetResult(true);
        }
    }

    /// <summary>
    /// Usued in OnRoomPropertiesUpdate to Compare keyValue with RPInProgress
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    void CompareWithRPInProgress(string key, object val)
    {
        if (rpInProgress.TryGetValue(key, out KeyValResultPair desiredKVRPair) && 
            ((desiredKVRPair.value == null && val == null) || val.Equals(desiredKVRPair.value)))
        {
            // successfully set to RoomProperties
            desiredKVRPair.setPropResult?.TrySetResult(true);
        }
    }

    private void NetworkingClientOnOpResponseReceived(OperationResponse opResponse)
    {
        Debug.Log($"NetworkingClientOnOpResponseReceived {opResponse.DebugMessage}");

        if (opResponse.OperationCode != OperationCode.SetProperties ||
            opResponse.ReturnCode != ErrorCode.InvalidOperation ||
            !opResponse.DebugMessage.Contains("CAS update failed")
            )
            return;

        var keyFromMessage = opResponse.DebugMessage?.Split('\'')[1];
        if (rpInProgress.TryGetValue(keyFromMessage, out KeyValResultPair desiredKVRPair))
        {
            // CAS failure
            desiredKVRPair.setPropResult?.TrySetResult(false);
        }
    }
    #endregion
}

[Serializable]
public class PlayerKey
{
    public Player player;
    public string key;

    public PlayerKey(Player pl, string ke)
    {
        player = pl;
        key = ke;
    }

    public override string ToString()
    {
        return player.UserId+"_"+key;
    }
}

[Serializable]
public class KeyValResultPair : KeyValExpPair
{
    public TaskCompletionSource<bool> setPropResult;

    public KeyValResultPair(string pairKey, object tarValue, object expValue):base(pairKey, tarValue, expValue)
    {
        setPropResult = new TaskCompletionSource<bool>();
    }
}



public enum RaiseEvnetCode: byte
{
    CustomManualInstantiationEventCode = 199
}