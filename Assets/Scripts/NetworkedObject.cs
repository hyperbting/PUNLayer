using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkedObject : MonoBehaviourPun,IPunOwnershipCallbacks
{
    void Start()
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    Debug.Log($"{photonView.}");
        //}
    }

    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
    {
        Debug.Log($"{photonView?.ViewID} {photonView?.Owner?.ActorNumber}");
    }
}
