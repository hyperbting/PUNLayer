using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class PUNConnecter : MonoBehaviourPunCallbacks
{
    #region Progress Record
    [SerializeField]
    private bool joiningRoom = false;
    [SerializeField]
    private bool leavingRoom = false;

    TaskCompletionSource<bool> joinRoomResult;
    TaskCompletionSource<bool> leaveRoomResult;

    [SerializeField]
    string lastJoinedRoom;
    #endregion

    #region MasterSserver to GameRoom
    public async Task<bool> JoinGameRoom(string roomName)
    {
        //Already in room?
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.Name == roomName)
            {
                Debug.Log($"{scriptName} JoinGameRoom AlreadyInNamedRoom");
                return true;
            }
            else
            {
                if (await LeaveRoom() && await IsConnectedAndReady(5000))
                {
                    Debug.Log($"{scriptName} JoinGameRoom LeftRoom And ReadyForJoinRoom");
                }
            }
        }

        if (joiningRoom)
        {
            Debug.LogWarning($"{scriptName} JoinGameRoom WasJoiningRoom");
            return await joinRoomResult.Task;
        }

        CurrentPhotonRoomState = PhotonRoomState.Connecting;

        // if for what reason NOT at state can JoinRoom, go offline
        if (!PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.OfflineMode = true;

        joiningRoom = true;

        var rooOpt = new RoomOptions() {
            PublishUserId = true
        };
        joinRoomResult = new TaskCompletionSource<bool>();

        if (string.IsNullOrEmpty(roomName))
            roomName = "Default";
        if (!PhotonNetwork.JoinOrCreateRoom(roomName, rooOpt, TypedLobby.Default)) // Will callback: OnJoinedRoom or OnJoinRoomFailed.
        {
            Debug.LogWarning($"{scriptName} JoinGameRoom JoinOrCreateRoom Immediately FAIL");
            joinRoomResult.TrySetResult(false);
        }

        //Wait until OnJoinedRoom or OnJoinRoomFailed
        await Task.WhenAny(joinRoomResult.Task, Task.Delay(60000));
        if(!joinRoomResult.Task.IsCompleted)
            joinRoomResult.TrySetResult(false);

        joiningRoom = false;

        return joinRoomResult.Task.Result;
    }
    #endregion

    #region Room Function
    public async Task<bool> LeaveRoom()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning($"{scriptName} LeaveRoom:NotInRoom");
            return true;
        }

        if (leavingRoom)
        {
            Debug.LogWarning($"{scriptName} LeaveRoom:WasLeavingRoom");
            return await leaveRoomResult.Task;
        }

        leavingRoom = true;
        leaveRoomResult = new TaskCompletionSource<bool>();
        PhotonNetwork.LeaveRoom();

        await Task.WhenAny(leaveRoomResult.Task, Task.Delay(60000));
        if (!leaveRoomResult.Task.IsCompleted)
            leaveRoomResult.TrySetResult(false);

        leavingRoom = false;
        return leaveRoomResult.Task.Result;
    }
    #endregion

    #region IMatchmakingCallbacks
    //public override void OnCreatedRoom()
    //{
    //    base.OnCreatedRoom();
    //}

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"{scriptName} OnCreateRoomFailed {returnCode} {message}");
        base.OnCreateRoomFailed(returnCode, message);

        OnCreateRoomFailedAction?.Invoke(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"{scriptName} OnJoinedRoom, {(PhotonNetwork.OfflineMode ? "Offline":"Online")}, " + PhotonNetwork.CurrentRoom.ToStringFull());
        base.OnJoinedRoom();

        OnJoinedRoomAction?.Invoke();

        joinRoomResult?.TrySetResult(true);

        lastJoinedRoom = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.OfflineMode)
        {
            CurrentPhotonRoomState = PhotonRoomState.OfflineRoom;
            incu.OnJoinedOfflineRoomEvent?.Invoke();
        }
        else
        {
            CurrentPhotonRoomState = PhotonRoomState.OnlineRoom;
            incu.OnJoinedOnlineRoomEvent?.Invoke();
        }
    }

    //void OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    // e.g. store this gameobject as this player's charater in Player.TagObject
    //    //info.sender.TagObject = this.GameObject;
    //}

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    base.OnJoinRandomFailed(returnCode, message);
    //}

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"{scriptName} OnJoinRoomFailed {returnCode} {message}");
        base.OnJoinRoomFailed(returnCode, message);

        OnJoinRoomFailedAction?.Invoke(returnCode, message);

        joinRoomResult?.TrySetResult(false);
    }

    public override void OnLeftRoom()
    {
        Debug.Log($"{scriptName} OnLeftRoom");
        base.OnLeftRoom();

        OnLeftRoomAction?.Invoke();

        leaveRoomResult?.TrySetResult(true);

        CurrentPhotonRoomState = PhotonRoomState.LeftRoom;
    }

    //public override void OnFriendListUpdate(List<FriendInfo> friendList)
    //{
    //    base.OnFriendListUpdate(friendList);
    //}
    #endregion

    public enum PhotonRoomState
    {
        Unknown,
        Connecting,
        CanJoinRoom,
        LeftRoom,
        OnlineRoom,
        OfflineRoom
    }
}