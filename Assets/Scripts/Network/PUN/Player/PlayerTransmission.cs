using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public partial class PlayerTransmission : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    static Transform tokenParent;
    [SerializeField]
    GameObject RefPlayer;

    public IPlayerMaker pm;
    public ISerializableHelper sh;

    private void Awake()
    {
        if (tokenParent == null)
            BuildTokenParent();

        transform.SetParent(tokenParent);
    }

    // for Owner
    List<SerilizableReadWrite> srw;
    public void Setup(List<SerilizableReadWrite> srw)
    {
        this.srw = srw;
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
