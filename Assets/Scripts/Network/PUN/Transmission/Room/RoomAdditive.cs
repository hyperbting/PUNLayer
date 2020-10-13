using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAdditive : MonoBehaviourPunCallbacks
{
    ITransmissionBase itb;

    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        this.itb = itb;
        if (photonView.IsMine)
        {
            //    gameObject.name = "MyPlayerToken";
            //    RefPlayer = pm.GetHostPlayer();
        }
        else
        {
            //gameObject.name = "RemotePlayerToken";
            //RefPlayer = pm.InstantiateRemotePlayerObject(photonView.Owner.UserId, gameObject.transform);

            //var istu = RefPlayer.GetComponent<ISyncTokenUser>();
            //istu.RegisterWithTransmissionToken(itb);
        }

        //SetupSync(data);
    }
}
