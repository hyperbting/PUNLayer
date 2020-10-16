using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAdditive : MonoBehaviourPunCallbacks
{
    public GameObject refObject;

    ITransmissionBase itb;

    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        this.itb = itb;

        //Load Prefab with InstantiationData data
        refObject = Load(data);

        if (photonView.IsMine)
        {
        }
        else
        {
            //// based on InstantiationData load proper prefab
            //SetupSync(data);

            //var istu = RefPlayer.GetComponent<ISyncTokenUser>();
            //istu.RegisterWithTransmissionToken(itb);
        }
    }

    GameObject Load(InstantiationData data)
    {

        return null;
    }
}
