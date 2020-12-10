using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;

public class RPSetting : MonoBehaviourPunCallbacks
{

    public GameObject HostPlayer;

    [SerializeField] TransformSyncType currentType = TransformSyncType.PhotonViewTransform;
    public TransformSyncType CurrentType {
        get {
            return currentType;
        }

        set {
            currentType = value;
            transformSyncTypeTEXT.text = value.ToString();
        }
    }

    ExitGames.Client.Photon.Hashtable tmpHT = new ExitGames.Client.Photon.Hashtable();

    public Text transformSyncTypeTEXT;
    private void Start()
    {
        if(PhotonNetwork.InRoom)
            SyncWithProp(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        SyncWithProp(propertiesThatChanged);
    }

    void SyncWithProp(ExitGames.Client.Photon.Hashtable prop)
    {
        foreach (var keyobj in prop.Keys)
        {
            if (Enum.TryParse((string)keyobj, out RPKey rpkey))
            {
                switch (rpkey)
                {
                    case RPKey.SyncType:
                        CurrentType = (TransformSyncType)prop[keyobj];
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        HostPlayer = PhotonNetwork.Instantiate("Test/BaseToken", Vector3.zero, Quaternion.identity);

        HostPlayer.GetComponent<RandomMove>().isOwner = true;

        SyncWithProp(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    #region TransformSyncType
    public void SyncTransformSyncType(TransformSyncType syncType)
    {
        if (!PhotonNetwork.InRoom)
            return;

        tmpHT.Clear();
        tmpHT[RPKey.SyncType.ToString()] = syncType;
        PhotonNetwork.CurrentRoom.SetCustomProperties(tmpHT);
    }
    #endregion
}

public enum RPKey
{
    SyncType,

    RandomMoveInterval,

}

public enum TransformSyncType
{
    None,
    SerializeView,
    PhotonViewTransform,
    PlayerProperties,
}
