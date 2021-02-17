using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class PUNConnecter : MonoBehaviourPunCallbacks
{
    #region Progress Record
    [SerializeField]
    private bool connectingToNameServer = false;
    [SerializeField]
    private bool connectingToMasterServer = false;
    [SerializeField]
    private bool disconnecting = false;

    TaskCompletionSource<bool> connectNSResult;
    TaskCompletionSource<bool> connectMSResult;
    TaskCompletionSource<bool> disconnectResult;

    //TaskCompletionSource<bool> connectLobbyResult;
    TaskCompletionSource<bool> leaveLobbyResult;
    TaskCompletionSource<bool> roomListUpdateResult;
    #endregion

    #region To NameServer
    async Task<bool> ConnectToNameServer()
    {
        if (connectingToNameServer)
            return await connectNSResult.Task;

        connectingToNameServer = true;
        connectNSResult = new TaskCompletionSource<bool>();

        SetupConnectSetting();
        if (!PhotonNetwork.NetworkingClient.ConnectToNameServer())
        {
            Debug.LogWarning("Immediate Fail to NameServer");
        }

        await Task.WhenAny(connectNSResult.Task, Task.Delay(30000));
        connectNSResult.TrySetResult(false);

        connectingToNameServer = false;
        return connectNSResult.Task.Result;
    }

    public async Task FetchRegionList(System.Action<RegionHandler> act = null)
    {
        // connect to NS to build RegionHandler
        if (PhotonNetwork.NetworkingClient == null || PhotonNetwork.NetworkingClient.RegionHandler == null)
        {
            Debug.LogWarning("FetchRegionList ConnectToNameServer");
            await ConnectToNameServer();
            //await Disconnect();

            // leave NS
            PhotonNetwork.Disconnect();
        }

        Debug.Log("UpdateRegionPing");
        // as long as RegionHandler exist, you can ping
        if (!UpdateRegionPing(out RegionPingResult upResult, act))
        {
        }

        // leave NS
        //for (int i = 0; i < 30; i++)
        //{
        //    await Task.Delay(100);
        //    if (PhotonNetwork.NetworkingClient.Server == ServerConnection.NameServer)
        //    {
        //        PhotonNetwork.NetworkingClient.Disconnect();
        //        break;
        //    }
        //}

        //connectingToNameServer = false;
    }
    #endregion

    #region All the way To MasterServer
    public void SetupConnectSetting()
    {
        try
        {
            PhotonNetwork.NickName = " ";
            PhotonNetwork.GameVersion = Application.version;
            PhotonNetwork.NetworkingClient.AppId = SerSettings.AppSettings.AppIdRealtime;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// 
    ///  Will Timeout after 60sec
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ConnectToServer()
    {
        //if already connected
        if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Already ConnectedToServer");
            return true;
        }

        CurrentPhotonRoomState = PhotonRoomState.Connecting;

        PhotonNetwork.OfflineMode = false;

        Debug.Log($"{scriptName} Connecting Server");

        if (connectingToMasterServer)
            return await connectMSResult.Task;

        SetupConnectSetting();

        connectingToMasterServer = true;
        connectMSResult = new TaskCompletionSource<bool>();

        #region Photon way to Connect MasterServer: ConnectToBestCloudServer/ ConnectToRegion/ ConnectToMaster
        switch (serMasterTarget.smTargetType)
        {
            case ServerTarget.ServerMasterTargetType.SpecificMaster:
                Debug.Log($"{scriptName} ConnectToMaster");
                //PhotonNetwork.ConnectToMaster(punMasterTarget.ipAddress, punMasterTarget.serverPort);
                break;
            case ServerTarget.ServerMasterTargetType.SpecificRegion:
                Debug.Log($"{scriptName} ConnectToRegion");
                PhotonNetwork.ConnectToRegion(serMasterTarget.photonRegion);
                break;
            case ServerTarget.ServerMasterTargetType.BestRegion:
            default:
                Debug.Log($"{scriptName} ConnectToCloud");
                PhotonNetwork.ConnectToBestCloudServer();
                break;
        }
        #endregion
        // Wait until either OnConnectedToMaster() or OnDisconnected
        await Task.WhenAny(connectMSResult.Task, Task.Delay(60000));
        if (!connectMSResult.Task.IsCompleted)
            connectMSResult.TrySetResult(false);

        if (!PhotonNetwork.IsConnectedAndReady)
            await Disconnect();

        connectingToMasterServer = false;
        return connectMSResult.Task.Result;
    }

    /// <summary>
    /// 
    /// Will Timeout after 60sec
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Disconnect()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning($"{scriptName} Disconnect:NotInServer");
            return true;
        }

        if (disconnecting)
        {
            Debug.LogWarning($"{scriptName} Disconnect:WasDisconnecting");
            return await disconnectResult.Task;
        }

        disconnecting = true;
        disconnectResult = new TaskCompletionSource<bool>();
        PhotonNetwork.Disconnect();

        await Task.WhenAny( disconnectResult.Task, Task.Delay(60000));
        if (!disconnectResult.Task.IsCompleted)
            disconnectResult.TrySetResult(false);

        disconnecting = false;
        return disconnectResult.Task.Result;
    }

    public async Task<bool> UpdateRoomList(TypedLobby typedLobby=null)
    {
        Debug.Log($"UpdateRoomList {PhotonNetwork.NetworkClientState}");

        //Join Lobby
        PhotonNetwork.JoinLobby(typedLobby);

        roomListUpdateResult = new TaskCompletionSource<bool>();

        //Wait until OnRoomListUpdate
        await Task.WhenAny(roomListUpdateResult.Task, Task.Delay(30000));
        if (!roomListUpdateResult.Task.IsCompleted)
            roomListUpdateResult?.TrySetResult(false);

        //Leave Lobby
        PhotonNetwork.LeaveLobby();

        //Wait Until LeftLobby
        leaveLobbyResult = new TaskCompletionSource<bool>();
        await Task.WhenAny(leaveLobbyResult.Task, Task.Delay(30000));
        if (!leaveLobbyResult.Task.IsCompleted)
            leaveLobbyResult?.TrySetResult(false);

        return (roomListUpdateResult.Task.Result && leaveLobbyResult.Task.Result);
    }
    #endregion

    #region ILobbyCallbacks
    public override void OnJoinedLobby()
    {
        Debug.Log($"{scriptName} OnJoinedLobby");
        base.OnJoinedLobby();
        //connectLobbyResult?.TrySetResult(true);
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();

        leaveLobbyResult?.TrySetResult(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log($"{scriptName} OnRoomListUpdate {roomList.Count}:" + roomList.ToStringFull<RoomInfo>());
        roomListUpdateResult?.TrySetResult(true);
    }

    //public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    //{
    //    base.OnLobbyStatisticsUpdate(lobbyStatistics);
    //}
    #endregion

    #region IConnectionCallbacks
    public override void OnConnected()
    {
        //Unused
        Debug.Log($"{scriptName} OnConnected {PhotonNetwork.CloudRegion}");
        base.OnConnected();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"{scriptName} OnConnectedToMaster");
        base.OnConnectedToMaster();
        CurrentPhotonRoomState = PhotonRoomState.CanJoinRoom;
        connectMSResult?.TrySetResult(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"{scriptName} OnDisconnected {cause}");
        base.OnDisconnected(cause);
        CurrentPhotonRoomState = PhotonRoomState.Unknown;

        connectMSResult?.TrySetResult(false);

        disconnectResult?.TrySetResult(true);
    }

    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log($"{scriptName} OnRegionListReceived");
        base.OnRegionListReceived(regionHandler);

        connectNSResult?.TrySetResult(true);
    }

    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        Debug.Log($"{scriptName} OnCustomAuthenticationResponse");
        base.OnCustomAuthenticationResponse(data);
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.LogWarning($"{scriptName} OnCustomAuthenticationFailed {debugMessage}");
        base.OnCustomAuthenticationFailed(debugMessage);
    }
    #endregion

    #region Setter
    #endregion
}