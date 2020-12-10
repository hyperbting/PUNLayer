﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializeViewPosRot : MonoBehaviour, IPunObservable
{
    public RandomMove rm;

    public bool SyncWithSerializeViewPosRot = false;
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!SyncWithSerializeViewPosRot)
            return;

        if (stream.IsWriting)
        {

            stream.SendNext(rm.targetPosition);
            stream.SendNext(rm.targetRotation);
        }
        else
        {
            rm.targetPosition = (Vector3)stream.ReceiveNext();
            rm.targetRotation = (Quaternion)stream.ReceiveNext();

            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            //rigidbody.position += rigidbody.velocity * lag;
        }
    }
}
