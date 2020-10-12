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
        Debug.Log($"TransmissionBase Start");
        var data = new InstantiationData(photonView.InstantiationData);
        switch (data.tokenType)
        {
            case SyncTokenType.Player:
                var pta = gameObject.AddComponent<PlayerTransmissionAdditive>();
                pta.Init(this);

                if (data.TryGetValue("syncPos", out string val) && val == "true")
                {
                    Debug.Log($"syncPos");
                    SeriHelper.Register(pta.BuildPosSync());
                }
                break;
            default:
                break;
        }

        if (photonView.IsMine)
        {
            Debug.Log($"I Own {photonView.ViewID} {PhotonNetwork.LocalPlayer.UserId} " + photonView.Owner.ToStringFull());
        }
        else
        {
            // deal with photonView.InstantiationData
            Debug.Log($"{photonView.ViewID} TryLoadData for {photonView.Owner.UserId}");
            Debug.Log($"InstantiationDataLength:{photonView.InstantiationData.Length}");
            for(int i = 0; i < photonView.InstantiationData.Length; i++)
                Debug.Log($"InstantiationData: {i} {photonView.InstantiationData[i]}");


        }

        started = true;
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
}
