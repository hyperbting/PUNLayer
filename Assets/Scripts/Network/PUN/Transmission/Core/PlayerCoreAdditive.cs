using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerCoreAdditive : MonoBehaviourPunCallbacks//, ITokenAdditive
{
    public GameObject RefPlayer;

    public IPlayerMaker pm;

    ITransmissionBase parent;

    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        this.parent = itb;

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

            var istu = RefPlayer.GetComponent<ISyncHandlerUser>();
            istu.SetupSync(itb, data);
        }

        SetupSync(data);
    }

    #region Sync Methods
    void SetupSync(InstantiationData data)
    {
        if (data.TryGetValue("syncPlayerPos", out string val) && val == "true")
        {
            parent.SeriHelper.Register(new SerializableReadWrite("SyncPos", ReadPos, WritePos));
        }

        if (data.TryGetValue("syncPlayerRot", out val) && val == "true")
        {
            parent.SeriHelper.Register(new SerializableReadWrite("SyncRot", ReadRot, WriteRot));
        }

        if (data.TryGetValue("syncPUNTrans", out val) && val == "true")
        {
            var scr = gameObject.AddComponent<TransformSubAdditive>();
            scr.RefTransform = RefPlayer.transform;
        }
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

    object ReadRot()
    {
        return RefPlayer.transform.rotation;
    }

    void WriteRot(object obj)
    {
        RefPlayer.transform.rotation = (Quaternion)obj;
    }
    #endregion

    public void UseTransformAdditive(Transform refTrans)
    {
        var scr = gameObject.AddComponent<TransformSubAdditive>();
        scr.RefTransform = refTrans;
    }
}
