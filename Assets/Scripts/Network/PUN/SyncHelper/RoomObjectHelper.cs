using Photon.Pun;
using UnityEngine;

public class RoomObjectHelper : MonoBehaviourPunCallbacks
{
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
    }

    #region
    public void InstantiateroomObject(InstantiationData insData)
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
            if (!insData.ContainsKey(InstantiationData.InstantiationKey.objectuuid))
            {
                var tokenID = $"ro_{Random.Range(1000, 9999).ToString()}";
                insData[InstantiationData.InstantiationKey.objectuuid.ToString()] = tokenID;
            }

            itp.RequestSyncToken(insData);
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
            InstantiateroomObject(tUser.SupplyInstantiationData);
        }
    }
}
