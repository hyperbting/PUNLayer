using System;
using System.Threading.Tasks;
using UnityEngine;

public interface INetworkConnect
{
    void Init(INetworkConnectUser incUser);

    #region Checker
    #endregion

    #region ToLobby
    /// <summary>
    /// ConnectToServer when service is connected and ready for Matchmaking
    /// </summary>
    /// <returns></returns>
    Task<bool> ConnectToServer();
    #endregion

    #region ToRoom
    /// <summary>
    /// JoinGameRoom
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    Task<bool> JoinGameRoom(string roomName);

    /// <summary>
    /// LeaveRoom
    /// </summary>
    /// <returns></returns>
    Task<bool> LeaveRoom();
    #endregion

    #region InRoom
    #region Checker
    bool IsInRoom();

    bool IsOnlineRoom();
    bool IsOfflineRoom();

    bool IsRoomOwner();
    #endregion
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

//public enum NetworkState
//{
//    Unknown,
//    Offline,    // In Offline Room  
//    Connecting, // from Offline to Online
//    Online,     // In Online Room
//}

//public enum InternetState
//{
//    Unknown,
//    NoInternet, // User Disable all Internet Adaptors
//    Reachable,
//}