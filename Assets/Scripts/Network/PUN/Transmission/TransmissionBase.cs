using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransmissionBase : MonoBehaviourPunCallbacks
{
    public ISerializableHelper sh;
    //public BaseSyncHelper 

    public bool started = false;
    protected virtual void Start()
    {
        sh = GetComponent<ISerializableHelper>();

        if (sh == null)
            Debug.LogWarning("sh NotFound");

        if (photonView.IsMine)
        {
            Debug.Log($"I Own {photonView.ViewID} {PhotonNetwork.LocalPlayer.UserId} " + photonView.Owner.ToStringFull());
        }
        else
        {
            Debug.Log($"{photonView.ViewID} TryLoadData for {photonView.Owner.UserId}" + photonView.InstantiationData);
        }

        started = true;

        //RegisterData();
    }

    // for Owner
    List<SerializableReadWrite> srw = new List<SerializableReadWrite>();
    public void Setup(List<SerializableReadWrite> srws)
    {
        srw = srws;
        Invoke("RegisterSerializableReadWrite", 0);
    }

    void RegisterSerializableReadWrite()
    {
        if (sh != null)
            sh.Register(srw.ToArray());
        else
            Invoke("RegisterSerializableReadWrite", 1);
    }

}
