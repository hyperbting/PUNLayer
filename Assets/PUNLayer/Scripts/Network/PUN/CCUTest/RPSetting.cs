using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Photon.Realtime;

public class RPSetting : MonoBehaviourPunCallbacks
{
    public static bool burst = false;
    public static int burstAmount = 100;

    public GameObject HostPlayer;

    [SerializeField] TransformSyncType currentType = TransformSyncType.SerializeViewCurrent;
    public TransformSyncType CurrentType {
        get {
            return currentType;
        }

        set {
            if (currentType == value)
                return;

            //clean log
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsReset();
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;

            //set/ display value
            currentType = value;
            transformSyncTypeTEXT.text = value.ToString();
        }
    }

    ExitGames.Client.Photon.Hashtable tmpHT = new ExitGames.Client.Photon.Hashtable();

    public Text transformSyncTypeTEXT;
    private void Start()
    {
        Application.targetFrameRate = 120;

        if(PhotonNetwork.InRoom)
            SyncWithProp(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public void OrderBurst()
    {
        if (!PhotonNetwork.InRoom)
            return;

        bool toBrust = false;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RPKey.Burst.ToString(), out object peoC))
        {
            toBrust = !(bool)peoC;
        }

        tmpHT.Clear();
        tmpHT[RPKey.Burst.ToString()] = toBrust;
        PhotonNetwork.CurrentRoom.SetCustomProperties(tmpHT);
    }

    public void SetupBurstAmount()
    {
        if (burstAmount == 50)
            burstAmount = 100;
        else if (burstAmount == 100)
            burstAmount = 200;
        else if (burstAmount == 200)
            burstAmount = 300;
        else if (burstAmount == 300)
            burstAmount = 600;
        else if (burstAmount == 600)
            burstAmount = 1000;
        else
            burstAmount = 50;

        tmpHT.Clear();
        tmpHT[RPKey.BurstAmount.ToString()] = burstAmount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(tmpHT);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        totalPeopleCount.text = CalculateTotalPeople().ToString();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        SyncWithProp(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        SyncWithProp(changedProps);
    }

    void SyncWithProp(ExitGames.Client.Photon.Hashtable prop)
    {
        foreach (var keyobj in prop.Keys)
        {
            if (Enum.TryParse((string)keyobj, out RPKey rpkey))
            {
                switch (rpkey)
                {
                    case RPKey.SyncType:
                        CurrentType = (TransformSyncType)prop[keyobj];
                        break;
                    case RPKey.PeoCount:
                        totalPeopleCount.text = CalculateTotalPeople().ToString();
                        break;
                    case RPKey.Burst:
                        burst = (bool)prop[keyobj];
                        break;
                    case RPKey.BurstAmount:
                        burstAmount = (int)prop[keyobj];
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public Text totalPeopleCount;
    public int CalculateTotalPeople()
    {
        if (!PhotonNetwork.InRoom)
            return -1;

        int result = 0;
        foreach (var pl in PhotonNetwork.CurrentRoom.Players)
            if (pl.Value.CustomProperties.TryGetValue(RPKey.PeoCount.ToString(), out object peoC))
                result += (int)peoC;

        return result;
    }

    public Text peopleCount;
    List<GameObject> npcs = new List<GameObject>();
    public (GameObject, int) InstantiateNPC()
    {
        if (!PhotonNetwork.InRoom)
            return (null, npcs.Count);

        var go = PhotonNetwork.Instantiate("Test/BaseToken", Vector3.zero, Quaternion.identity);
        go.GetComponent<RandomMove>().isOwner = true;
        npcs.Add(go);
        peopleCount.text = npcs.Count.ToString();

        tmpHT.Clear();
        tmpHT[RPKey.PeoCount.ToString()] = npcs.Count+1;
        PhotonNetwork.LocalPlayer.SetCustomProperties(tmpHT);

        return (go, npcs.Count);
    }

    public bool DestroyNPC()
    {
        if (!PhotonNetwork.InRoom)
            return false;
        if (npcs == null || npcs.Count <=0)
            return false;

        foreach (var npcgo in npcs)
            PhotonNetwork.Destroy(npcgo);

        peopleCount.text = "0";

        tmpHT.Clear();
        tmpHT[RPKey.PeoCount.ToString()] = 1;
        PhotonNetwork.LocalPlayer.SetCustomProperties(tmpHT);
        npcs.Clear();
        return true;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        HostPlayer = PhotonNetwork.Instantiate("Test/BaseToken", Vector3.zero, Quaternion.identity);

        HostPlayer.GetComponent<RandomMove>().isOwner = true;

        tmpHT.Clear();
        tmpHT[RPKey.PeoCount.ToString()] = 1;
        PhotonNetwork.LocalPlayer.SetCustomProperties(tmpHT);

        if (PhotonNetwork.IsMasterClient)
        {
            SyncTransformSyncType(TransformSyncType.SerializeViewCurrent);
            SyncBrust(burst);
            SyncBrustMode(burstAmount);
        }
        else
        {
            SyncWithProp(PhotonNetwork.CurrentRoom.CustomProperties);
        }
    }

    #region TransformSyncType
    public void SyncTransformSyncType(TransformSyncType syncType)
    {
        if (!PhotonNetwork.InRoom)
            return;

        tmpHT.Clear();
        tmpHT[RPKey.SyncType.ToString()] = syncType;
        PhotonNetwork.CurrentRoom.SetCustomProperties(tmpHT);
    }
    #endregion

    public void SyncBrust(bool enabled)
    {
        if (!PhotonNetwork.InRoom)
            return;

        tmpHT.Clear();
        tmpHT[RPKey.Burst.ToString()] = enabled;
        PhotonNetwork.CurrentRoom.SetCustomProperties(tmpHT);
    }

    public void SyncBrustMode(int mesSize)
    {
        if (!PhotonNetwork.InRoom)
            return;

        tmpHT.Clear();
        tmpHT[RPKey.BurstAmount.ToString()] = mesSize;
        PhotonNetwork.CurrentRoom.SetCustomProperties(tmpHT);
    }
}

public enum RPKey
{
    SyncType,

    RandomMoveInterval,
    PeoCount,

    Burst,
    BurstAmount,
}

public enum TransformSyncType
{
    None,
    SerializeViewCurrent,
    SerializeViewTargetOnly,
    //PhotonViewTransform,
    PlayerProperties,
}
