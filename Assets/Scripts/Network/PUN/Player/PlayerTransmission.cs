using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public partial class PlayerTransmission : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField]
    GameObject RefPlayer;

    public IPlayerMaker pm;
    public ISerializableHelper sh;

    private void Awake()
    {
    }

    // for Owner
    List<SerializableReadWrite > srw = new List<SerializableReadWrite>();
    public void Setup(List<SerializableReadWrite> srws)
    {
        srw = srws;
        Invoke("RegisterSerializableReadWrite",0);
    }

    void RegisterSerializableReadWrite()
    {
        if (sh != null)
            sh.Register(srw.ToArray());
        else
            Invoke("RegisterSerializableReadWrite",1);
    }


    //void RegisterData()
    //{
    //    if (srw == null)
    //        return;

    //    foreach (var rw in srw)
    //    {
    //        sh.Register(rw);
    //    }
    //    Debug.Log($"Register {srw.Count} for Photon:{photonView.OwnerActorNr}");
    //    srw.Clear();
    //}

    public bool started = false;
    private void Start()
    {
        pm = GameObject.Find("PlayerManager").GetComponent<IPlayerMaker>();
        sh = GetComponent<ISerializableHelper>();

        if (pm == null)
            Debug.LogWarning("pm NotFound");

        if (sh == null)
            Debug.LogWarning("sh NotFound");

        if (photonView.IsMine)
        {
            gameObject.name = "MyPlayerToken";
            RefPlayer = pm.GetHostPlayer();
            Debug.Log($"I Own {photonView.ViewID} {PhotonNetwork.LocalPlayer.UserId} " + photonView.Owner.ToStringFull());
        }
        else
        {
            gameObject.name = "RemotePlayerToken";
            Debug.Log($"{photonView.ViewID} TryLoadData for {photonView.Owner.UserId}" + photonView.InstantiationData);
            RefPlayer = pm.InstantiateRemotePlayerObject(photonView.Owner.UserId);

            //follower = transform;
            RefPlayer.transform.SetParent(transform);
        }

        started = true;

        //RegisterData();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
    }
}
