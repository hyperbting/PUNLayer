using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializeViewPosRot : MonoBehaviour, IPunObservable
{
    public RandomMove rm;
    public Transform meshContainer;

    public bool SyncWithSerializeViewPosRot = false;
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!SyncWithSerializeViewPosRot)
            return;

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(meshContainer.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            meshContainer.rotation = (Quaternion)stream.ReceiveNext();

            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            //rigidbody.position += rigidbody.velocity * lag;
        }
    }
}
