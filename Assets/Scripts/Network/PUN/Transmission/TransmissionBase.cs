using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransmissionBase : MonoBehaviourPunCallbacks, ITransmissionBase
{
    [SerializeField] SerializableHelper seriHelper;
    public SerializableHelper SeriHelper
    {
        get
        {
            if(seriHelper == null)
                seriHelper = GetComponent<SerializableHelper>();
            return seriHelper;
        }
    }

    [SerializeField] StateHelper statHelper;
    public StateHelper StatHelper
    {
        get
        {
            if (statHelper == null)
                statHelper = GetComponent<StateHelper>();
            return statHelper;
        }

    }

    public bool started = false;
    protected virtual void Start()
    {
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
        if (seriHelper != null)
            seriHelper.Register(srw.ToArray());
        else
            Invoke("RegisterSerializableReadWrite", 1);
    }

}
