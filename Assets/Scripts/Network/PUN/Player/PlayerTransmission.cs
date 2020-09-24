using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public partial class PlayerTransmission : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    static Transform tokenParent;

    public IPlayerMaker pm;
    public ISerializableHelper sh;

    private void Awake()
    {
        if (tokenParent == null)
            BuildTokenParent();

        transform.SetParent(tokenParent);
    }

    //private void OnDestroy()
    //{
    //    sh.Unregister(new SerilizableReadWrite(ReadPosition, WritePosition) { name = "SyncPos" });
    //    sh.Unregister(new SerilizableReadWrite(ReadRotation, WriteRotation) { name = "SyncRot" });
    //}

    private void Start()
    {
        pm = GameObject.Find("PlayerManager").GetComponent<IPlayerMaker>();
        sh = GetComponent<ISerializableHelper>();

        if (photonView.IsMine)
        {
            gameObject.name = "MyPlayerToken";
            Debug.Log($"I Own {photonView.ViewID} {PhotonNetwork.LocalPlayer.UserId} " + photonView.Owner.ToStringFull());
        }
        else
        {
            gameObject.name = "RemotePlayerToken";
            Debug.Log($"{photonView.ViewID} TryLoadData for {photonView.Owner.UserId}" + photonView.InstantiationData);
            var go = pm.InstantiateRemotePlayerObject(photonView.Owner.UserId);

            //follower = transform;
            go.transform.SetParent(transform);
        }
    }

    // for Owner
    public void Setup(List<SerilizableReadWrite> srw)
    {
        foreach (var rw in srw)
        {
            sh.Register(rw);
        }
    }

    void BuildTokenParent()
    {
        var go = GameObject.Find("TokenParent");
        if (go != null)
            tokenParent = go.transform;
        else
            tokenParent = new GameObject("TokenParent").transform;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
    }
}
