using Photon.Pun;
using UnityEngine;

/// <summary>
/// Maintainer
/// </summary>
public class RoomObjectHelper : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject objectSupplyManager;
    IObjectSupplyManager iosManager;

    ITokenProvider itp;
    [SerializeField] GameObject roomObjectRoot;
    public GameObject RoomObjectRoot
    {
        get
        {
            if (roomObjectRoot == null)
                roomObjectRoot = new GameObject("RoomObjectRoot");

            return roomObjectRoot;
        }
    }

    private void Awake()
    {
        itp = GetComponentInParent<ITokenProvider>();
        iosManager = objectSupplyManager.GetComponent<IObjectSupplyManager>();
    }

    #region
    public void InstantiateRoomObject(InstantiationData insData)
    {
        photonView.RPC("RequestRoomObjectManipulation", RpcTarget.MasterClient, insData.ToData() as object);
    }

    public void DestroyRoomObject(InstantiationData insData)
    {
        photonView.RPC("RequestRoomObjectManipulation", RpcTarget.MasterClient, insData.ToData() as object);
    }

    [PunRPC]
    public void RequestRoomObjectManipulation(object data, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[RequestRoomObjectManipulation] Only Master Client can Manipulate RoomObject!");
            return;
        }

        var insData = new InstantiationData(data as object[]);

        //TODO: MC Verify data
        if (insData.tokenType != SyncTokenType.General)
        {
            Debug.LogWarning("[RequestRoomObjectManipulation] Only For Manipulate RoomObject!");
            return;
        }

        if (insData.TryGetValue(InstantiationData.InstantiationKey.objectname, out object objName))
        {
            // Object UUID
            if (!insData.ContainsKey(InstantiationData.InstantiationKey.objectuuid))
            {
                var tokenID = $"ro_{Random.Range(1000, 9999).ToString()}";
                insData[InstantiationData.InstantiationKey.objectuuid.ToString()] = tokenID;
            }

            if (insData.TryGetValue(InstantiationData.InstantiationKey.sceneobject, out object soModification))
            {
                switch ((string)soModification)
                {
                    case "create":
                        Debug.LogWarning("[RequestRoomObjectManipulation] Create");
                        itp.RequestSyncToken(insData);
                        break;
                    case "destroy":
                        photonView.RPC("RequestLocalObjectManipulation", RpcTarget.Others, insData.ToData() as object);

                        itp.RevokeSyncToken(insData);

                        iosManager.DestroyObject((string)objName, (string)insData["objectuuid"]);

                        Debug.LogWarning($"[RequestRoomObjectManipulation] MC Destroy {insData}");
                        break;
                }
            }            
        }
    }

    [PunRPC]
    public void RequestLocalObjectManipulation(object data, PhotonMessageInfo info)
    {
        var insData = new InstantiationData(data as object[]);
        if (insData.tokenType != SyncTokenType.General)
        {
            Debug.LogWarning("[RequestLocalObjectManipulation] Only For Manipulate Local RoomObject!");
            return;
        }

        if (insData.TryGetValue("localobject", out object objName))
        {
            if (!insData.ContainsKey("objectuuid"))
            {
                return;
            }

            if (insData.TryGetValue("sceneobject", out object modifyword) && (string)modifyword == "Destroy")
            {
                iosManager.DestroyObject((string)objName, (string)insData["objectuuid"]);
            }
        }
    }

    #endregion

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        base.OnJoinedRoom();

        var tUsers = RoomObjectRoot.GetComponentsInChildren<ISyncHandlerUser>();
        foreach (var tUser in tUsers)
        {
            Debug.Log($"Insert {tUser.SupplyInstantiationData} Into Room");
            InstantiateRoomObject(tUser.SupplyInstantiationData);
        }
    }
}
