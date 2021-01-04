using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistExistenceHandler : MonoBehaviourPunCallbacks
{
    readonly string PlayerPropUUIDKey = "PpUUID";

    ITransmissionBase parent;
    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        parent = itb;

        //pm = GameObject.Find("PlayerManager").GetComponent<IPlayerMaker>();
        //if (pm == null)
        //    Debug.LogWarning("pm NotFound");

        //if (photonView.IsMine)
        //{
        //    gameObject.name = "MyPlayerToken";
        //    RefPlayer = pm.GetHostPlayer();
        //}
        //else
        //{
        //    gameObject.name = "RemotePlayerToken";
        //    RefPlayer = pm.InstantiateRemotePlayerObject(photonView.Owner.UserId, gameObject.transform);

        //    var istu = RefPlayer.GetComponent<ISyncHandlerUser>();
        //    istu.SetupSync(itb, data);
        //}

        //SetupSync(data);

        this.enabled = true;
    }

    #region PunCallbacks
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        //someone joined, check if I have to give mine to.
        if (newPlayer.CustomProperties.TryGetValue(PlayerPropUUIDKey, out object id))
        {
            //Cancel Destroy if any
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log($"[PersistExistenceHandler] OnPlayerLeftRoom owner:{photonView.Owner} actnr:{photonView.OwnerActorNr}");
        if (otherPlayer.ActorNumber != photonView.ControllerActorNr)
            return;

        //if mine owner left, countdown to destroy
    }
    #endregion

    #region Countdown Destroy

    #endregion
}
