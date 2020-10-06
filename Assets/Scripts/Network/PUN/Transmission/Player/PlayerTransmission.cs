using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public partial class PlayerTransmission : TransmissionBase, IPunInstantiateMagicCallback
{
    [SerializeField]
    GameObject RefPlayer;

    public IPlayerMaker pm;

    protected override void Start()
    {
        base.Start();

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
            RefPlayer = pm.InstantiateRemotePlayerObject(photonView.Owner.UserId);
            RefPlayer.transform.SetParent(transform);
        }

        //RegisterData();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
    }
}
