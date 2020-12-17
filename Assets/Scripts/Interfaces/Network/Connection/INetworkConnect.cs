using System;
using System.Threading.Tasks;
using UnityEngine;

public interface INetworkConnect
{
    void Init(INetworkConnectUser incUser);

    #region Checker
    #endregion

    #region ToLobby
    // notify when service is connected and ready for Matchmaking
    Task<bool> ConnectToServer();
    #endregion

    #region ToRoom
    // notify when Matchmaking success
    Task<bool> JoinGameRoom(string roomName);
    #endregion

    #region InRoom
    #region Checker
    bool IsInRoom();

    bool IsOnlineRoom();
    bool IsOfflineRoom();

    bool IsRoomOwner();
    #endregion

    GameObject RequestSyncToken(InstantiationData dataToSend, Transform refTrasnform);
    GameObject ManualBuildSyncToken(InstantiationData dataToSend);
    //Task<bool> SetRoomProperty(KeyValExpPair kvePair);
    //Task<bool> SetPlayerProperty(Player player, KeyValExpPair kvePair);
    #endregion
}

/// <summary>
/// INetworkConnectUser have to provide some callbacks for subscription
/// </summary>
public interface INetworkConnectUser
{
    Action OnJoinedOnlineRoomEvent { get; set; }
    Action OnJoinedOfflineRoomEvent { get; set; }
}

public enum NetworkState
{
    Unknown,
    Offline,    // In Offline Room  
    Connecting, // from Offline to Online
    Online,     // In Online Room
}

public enum InternetState
{
    Unknown,
    NoInternet, // User Disable all Internet Adaptors
    Reachable,
}