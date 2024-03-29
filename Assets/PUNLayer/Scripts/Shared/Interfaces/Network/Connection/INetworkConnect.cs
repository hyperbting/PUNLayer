﻿using System;
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
    bool IsInRoom(OnOffline ooline=OnOffline.Any, string requestedRoomName="");

    bool IsRoomOwner();
    #endregion
    #region Getter
    int GetNetworkID();
    #endregion
    #endregion
    }

public enum OnOffline
{
    Any,
    Online,
    Offline,
}

/// <summary>
/// INetworkConnectUser have to provide some callbacks for subscription
/// </summary>
public interface INetworkConnectUser
{
    Action<OnOffline> OnJoinedRoomEvent { get; set; }
}
