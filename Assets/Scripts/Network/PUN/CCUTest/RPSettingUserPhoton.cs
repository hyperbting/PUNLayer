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
        nameTag.text = photonView.ViewID.ToString();

        if (photonView.IsMine)
        {
            gameObject.name += "(mine)";
            nameTag.color = Color.red;
        }
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
                object vall;
                switch (rpkey)
                {
                    case RPKey.SyncType:
                        //Debug.Log($"SyncType {rProperties[keyobj]}");
                        vall = TransformSyncType.SerializeViewCurrent;
                        rProperties.TryGetValue(keyobj, out vall);

                        SetTransformSyncType((TransformSyncType)vall);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void SetTransformSyncType(TransformSyncType syncType)
    {
        var serPosRot = GetComponent<SerializeViewPosRot>();
        var serTarget = GetComponent<SerializeViewTargetOnly>();

        var move = GetComponent<RandomMove>();

        serPosRot.SyncWithSerializeViewPosRot = false;
        serTarget.SyncWithSerializeViewTarget = false;

        rm.lerpToTarget = false;

        switch (syncType)
        {
            case TransformSyncType.SerializeViewTargetOnly:
                serTarget.SyncWithSerializeViewTarget = true;
                rm.lerpToTarget = true;
                break;
            case TransformSyncType.SerializeViewCurrent:
                serPosRot.SyncWithSerializeViewPosRot = true;
                break;
            default:
            case TransformSyncType.None:
                break;
        }
    }
}
