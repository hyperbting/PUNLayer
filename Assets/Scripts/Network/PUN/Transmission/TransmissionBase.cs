using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransmissionBase : MonoBehaviourPunCallbacks, ITransmissionBase
{
    #region Properties
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
    #endregion

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

    #region Register
    public void Register(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
        {
            switch (srw.syncType)
            {
                case SyncHelperType.RoomState:
                case SyncHelperType.PlayerState:
                    statHelper.Register(srw);
                    break;
                case SyncHelperType.Serializable:
                    seriHelper.Register(srw);
                    break;
                default:
                    break;
            }
        }
    }

    public void Unregister(params SerializableReadWrite[] srws)
    {
        foreach (var srw in srws)
        {
            switch (srw.syncType)
            {
                case SyncHelperType.RoomState:
                case SyncHelperType.PlayerState:
                    statHelper.Unregister(srw.name);
                    break;
                case SyncHelperType.Serializable:
                    seriHelper.Unregister(srw.name);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region Use SerializableHelper/ StateHelper
    public void UpdateProperties(SyncTokenType stType, string key, object data)
    {
        switch (stType)
        {
            case SyncTokenType.Player:
                statHelper.UpdateProperties(key, data, SyncTokenType.Player);
                break;
            default:
            case SyncTokenType.General:
                statHelper.UpdateProperties(key, data, SyncTokenType.General);
                break;
        }
    }
    #endregion

    void RegisterSerializableReadWrite()
    {
        if (seriHelper != null)
            seriHelper.Register(srw.ToArray());
        else
            Invoke("RegisterSerializableReadWrite", 1);
    }

}
