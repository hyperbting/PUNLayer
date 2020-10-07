using System;
using System.Threading.Tasks;
using UnityEngine;

public interface INetworkConnect
{
    void Init(INetworkConnectUser incUser);

    #region Checker
    //bool IsOwner();
    #endregion

    // notify when service is connected and ready for Matchmaking
    Task<bool> ConnectToServer();

    // notify when Matchmaking success
    Task<bool> JoinGameRoom(string roomName);

    #region InRoom
    bool IsOnlineRoom();
    bool IsOfflineRoom();

    GameObject RequestSyncToken(SyncTokenType syncTokenType, Transform refTrasnform);
    //Task<bool> SetRoomProperty(KeyValExpPair kvePair);
    //Task<bool> SetPlayerProperty(Player player, KeyValExpPair kvePair);
    #endregion
}

public interface INetworkConnectUser
{
    Action OnJoinedRoomEvent { get; set; }
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