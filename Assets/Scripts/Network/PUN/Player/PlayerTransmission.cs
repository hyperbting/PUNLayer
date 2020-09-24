using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public partial class PlayerTransmission : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField]
    private Transform follower;

    static Transform tokenParent;

    public IPlayerMaker pm;
    public ISerializableHelper sh;

    private void Awake()
    {
        if (tokenParent == null)
            BuildTokenParent();

        transform.SetParent(tokenParent);
    }

    private void OnDestroy()
    {
        sh.Unregister(new SerilizableReadWrite(ReadPosition, WritePosition) { name = "SyncPos" });
        sh.Unregister(new SerilizableReadWrite(ReadRotation, WriteRotation) { name = "SyncRot" });
    }

    private void Start()
    {
        sh = GetComponent<ISerializableHelper>();
        sh.Register(new SerilizableReadWrite(ReadPosition, WritePosition) { name="SyncPos"});
        sh.Register(new SerilizableReadWrite(ReadRotation, WriteRotation) { name ="SyncRot"});

        pm = GameObject.Find("PlayerManager").GetComponent<IPlayerMaker>();

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

            follower = transform;
            go.transform.SetParent(transform);
        }

        // Now Deal with Personal Items
        pm.SyncPersonalItems();
    }

    // for Owner
    public void Setup(Transform fol)
    {
        //follow my own Player
        follower = fol;
        //_ = PUNConnecter.Instance.SetPlayerProperty(PhotonNetwork.LocalPlayer, new KeyValExpPair("uuid", PhotonNetwork.NickName));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransform();
    }

    void UpdateTransform()
    {
        if (follower == null)
            return;

        transform.position = follower.position;
        transform.rotation = follower.rotation;
    }

    #region Serializable Sync
    object ReadPosition()
    {
        //Debug.Log($"ReadPosition");

        if (follower == null)
            return null;

        //Debug.Log($"ReadPosition {follower.position}");
        return follower.position;
    }

    void WritePosition(object obj)
    {
        //Debug.Log($"WritePosition");

        if (obj == null)
            return;

        transform.position = (Vector3)obj;
        //Debug.Log($"WritePosition {(Vector3)obj}");
    }

    object ReadRotation()
    {
        if (follower != null)
            return follower.rotation;
        return null;
    }

    void WriteRotation(object obj)
    {
        if (obj != null)
            transform.rotation = (Quaternion)obj;
    }
    #endregion

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
        //pm = photonView.InstantiationData[0] as IPlayerMaker;
    }
}
