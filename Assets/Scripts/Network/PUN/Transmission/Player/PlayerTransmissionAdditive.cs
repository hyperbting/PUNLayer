using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransmissionAdditive : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject RefPlayer;

    public IPlayerMaker pm;

    public void Init(ITransmissionBase itb)
    {
        //// PlayerTransmission using PhotonTransformView to Sync Pos and Rot
        //var ptv = gameObject.AddComponent<PhotonTransformView>();
        //photonView.ObservedComponents.Add(ptv);

        pm = GameObject.Find("PlayerManager").GetComponent<IPlayerMaker>();
        if (pm == null)
            Debug.LogWarning("pm NotFound");

        if (photonView.IsMine)
        {
            gameObject.name = "MyPlayerToken";
            RefPlayer = pm.GetHostPlayer();
        }
        else
        {
            gameObject.name = "RemotePlayerToken";
            RefPlayer = pm.InstantiateRemotePlayerObject(photonView.Owner.UserId, gameObject.transform);

            var istu = RefPlayer.GetComponent<ISyncTokenUser>();
            istu.RegisterWithTransmissionToken(itb);
        }
    }

    public SerializableReadWrite BuildPosSync()
    {
        return new SerializableReadWrite( "SyncPos", ReadPos, WritePos );
    }

    object ReadPos()
    {
        //Debug.LogWarning("Read Pos");
        return RefPlayer.transform.position;
    }

    void WritePos(object obj)
    {
        //Debug.LogWarning("Write Pos");
        RefPlayer.transform.position = (Vector3)obj;
    }
}
