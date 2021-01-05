using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Receive RaiseEvent
/// Check if Specific PersistExistenceAdditive-id is created
/// assign created PersistExistenceAdditive to specific owner
/// if not create one PersistExistenceAdditive and Register
/// </summary>
public class PersistExistenceHandler: MonoBehaviourPunCallbacks, IOnEventCallback
{
    public readonly byte PlayerPersistExistence = 198;
    public readonly string PlayerPropUUIDKey = "PpUUID";

    Dictionary<string, PersistExistenceAdditive> dic = new Dictionary<string, PersistExistenceAdditive>();

    public ITokenProvider tokenProvider;

    public void Unregister(string uuid)
    {
        if (dic.TryGetValue(uuid, out PersistExistenceAdditive target))
        {
            //target.Destroy();
            //dic.Remove(uuid);
        }
    }

    #region RaiseEvent To MC Create ROOM OBJECT
    //paired with void IOnEventCallback.OnEvent(EventData photonEvent)
    public void CreatePersistExistence(InstantiationData data)
    {
        //TODO: ceationData
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {
            Receivers = ReceiverGroup.MasterClient,
            CachingOption = EventCaching.DoNotCache
        };

        PhotonNetwork.RaiseEvent(PlayerPersistExistence, data, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion

    #region PunCallbacks
    // Destroy ALL? when left room
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        foreach (var kvp in dic)
        {
            //Clean mine immediately
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropUUIDKey, out object uuid) && 
                kvp.Key == (string)uuid)
            {
                Unregister(kvp.Key);
                continue;
            }

            //Start CountdownDestroy OTHERs'
            kvp.Value.StartCountdown();
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log($"[PersistExistenceAdditive] OnPlayerLeftRoom owner:{photonView.Owner} actnr:{photonView.OwnerActorNr}");
        if (otherPlayer.CustomProperties.TryGetValue(PlayerPropUUIDKey, out object uuid) &&
            dic.TryGetValue((string)uuid, out PersistExistenceAdditive target))
        {
            //if owner left, countdown to destroy
            target.StartCountdown();
        }            
    }

    // create or give to sender
    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (photonEvent.Code != PlayerPersistExistence)
            return;

        Debug.Log($"{photonEvent.Sender} raised event");
        var punPlayer = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
        if (punPlayer == null /*&& !punPlayer.IsInactive*/)
        {
            Debug.Log($"{photonEvent.Sender}: no such InRoomPlayer");
            return;
        }

        PersistExistenceAdditive target = null;
        //TryGive from dic
        if (punPlayer.CustomProperties.TryGetValue(PlayerPropUUIDKey, out object uuid) &&
            dic.TryGetValue((string)uuid, out target) &&
            target.IsSignatureMatch(photonView.InstantiationData)
            )
        {
            //Cancel Countdown
            target.CancelCountdown();
            
            //Update PhotonViewID
        }
        else
        {
            var insData = new InstantiationData((object[])photonEvent.CustomData);
            //create one for targetPlayer
            var trasnTokenGO = tokenProvider.RequestManualSyncToken(insData) as GameObject;
            target = trasnTokenGO.GetComponent<PersistExistenceAdditive>();
        }

        //transferwonership
        if (target != null)
            target.photonView.TransferOwnership(punPlayer);
    }
    #endregion
}
