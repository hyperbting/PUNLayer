using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPSettingUserPhoton : MonoBehaviourPunCallbacks
{
    public Text nameTag;
    public RandomMove rm;
    public void Start()
    {
        Setup(PhotonNetwork.CurrentRoom.CustomProperties);
        nameTag.text = photonView.OwnerActorNr.ToString();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        Setup(propertiesThatChanged);
    }

    public void Setup(ExitGames.Client.Photon.Hashtable rProperties)
    {
        foreach (var keyobj in rProperties.Keys)
        {
            if (Enum.TryParse((string)keyobj, out RPKey rpkey))
            {
                switch (rpkey)
                {
                    case RPKey.SyncType:
                        //Debug.Log($"SyncType {rProperties[keyobj]}");
                        SetTransformSyncType((TransformSyncType)rProperties[keyobj]);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void SetTransformSyncType(TransformSyncType syncType)
    {
        var ptv = GetComponent<PhotonTransformView>();
        var ser = GetComponent<SerializeViewPosRot>();

        var move = GetComponent<RandomMove>();

        ptv.enabled = false;
        ser.SyncWithSerializeViewPosRot = false;
        rm.usingSerializeView = false;
        switch (syncType)
        {
            case TransformSyncType.PhotonViewTransform:
                ptv.enabled = true;
                break;
            case TransformSyncType.SerializeView:
                ser.SyncWithSerializeViewPosRot = true;
                rm.usingSerializeView = true;
                break;
            default:
            case TransformSyncType.None:
                break;
        }
    }
}
