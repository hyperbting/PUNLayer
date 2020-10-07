using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public partial class PUNConnecter : MonoBehaviourPunCallbacks, INetworkConnect
{
    public static PUNConnecter Instance { get; private set; } = null;
    public INetworkConnectUser incu;

    private readonly string scriptName = "PUNConnecter";
    public ServerSettings serSettings;

    #region
    Action<PhotonRoomState, PhotonRoomState> OnPhotonRoomStateChange;
    [SerializeField]
    PhotonRoomState currentPhotonRoomState = PhotonRoomState.Unknown;
    public PhotonRoomState CurrentPhotonRoomState {
        get {
            return currentPhotonRoomState;
        }
        set {
            if (currentPhotonRoomState == value)
                return;

            Debug.Log($"{scriptName} PhotonRoomState Shift From {currentPhotonRoomState} to {value}");
            OnPhotonRoomStateChange?.Invoke(currentPhotonRoomState, value);
            currentPhotonRoomState = value;
        }
    }

    public ServerTarget serMasterTarget;
    #endregion
    public void Init(INetworkConnectUser incUser)
    {
        incu = incUser;
    }

    #region PUN OnCallbackAction
    public Action OnJoinedRoomAction;
    public Action OnLeftRoomAction;
    public Action<short, string> OnCreateRoomFailedAction;
    public Action<short, string> OnJoinRoomFailedAction;
    public Action<Player> OnPlayerEnteredRoomAction;
    public Action<Player> OnPlayerLeftRoomAction;

    /// <summary>
    /// Key, Value
    /// </summary>
    public Action<string, object> OnRoomPropertyUpdateAction;
    public Action<Player, string, object> OnPlayerPropertyUpdateAction;
    #endregion

    #region Mono
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.OpResponseReceived += NetworkingClientOnOpResponseReceived;

        OnPlayerPropertyUpdateAction += CompareWithPPInProgress;
        OnRoomPropertyUpdateAction += CompareWithRPInProgress;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.OpResponseReceived -= NetworkingClientOnOpResponseReceived;

        OnPlayerPropertyUpdateAction -= CompareWithPPInProgress;
        OnRoomPropertyUpdateAction -= CompareWithRPInProgress;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void FixedUpdate()
    {
        if (!IsInvoking("BackgroundRunner"))
            InvokeRepeating("BackgroundRunner", 1, 1);
    }

    void BackgroundRunner()
    {
    }
    #endregion

    public async Task<bool> JoinOnlineRoom(string targetRoomName)
    {
        if (PhotonNetwork.OfflineMode)
        {
            if (!await ConnectToServer())
            {
                return false;
            }
        }

        if (!await JoinGameRoom(targetRoomName))
        {
            return false;
        }

        return CurrentPhotonRoomState == PhotonRoomState.OnlineRoom;
    }

    public async void Offline()
    {
        if (PhotonNetwork.InRoom)
            await LeaveRoom();

        if (PhotonNetwork.IsConnectedAndReady)
            await Disconnect();

        PhotonNetwork.OfflineMode = true;
    }

    public enum RegionPingResult
    {
        Unknown,
        NotReady,
        OnGoing,
        StillPinging,
    }

    public bool UpdateRegionPing(out RegionPingResult result, Action<RegionHandler> act = null)
    {
        result = RegionPingResult.Unknown;
        if (PhotonNetwork.NetworkingClient == null || PhotonNetwork.NetworkingClient.RegionHandler == null)
        {
            result = RegionPingResult.NotReady;
            return false;
        }

        var regHandler = PhotonNetwork.NetworkingClient.RegionHandler;
        if (regHandler.IsPinging)
        {
            result = RegionPingResult.StillPinging;
            return false;
        }

        regHandler.PingMinimumOfRegions(
            (RegionHandler rh) =>
            {
                Debug.Log(rh.EnabledRegions.Count + " " +rh.BestRegion.Code+ " from " + rh.EnabledRegions.ToStringFull());
                act?.Invoke(rh);
            },
            regHandler.SummaryToCache
        );

        result = RegionPingResult.OnGoing;
        return true;
    }

    #region NetworkLayer
    #region Checker
    async Task<bool> IsConnectedAndReady(int timeoutMS)
    {
        for (int i = 0; i < timeoutMS / 100; i++)
        {
            await Task.Delay(100);
            if (PhotonNetwork.IsConnectedAndReady)
                return true;
        }

        Debug.LogWarning($"{scriptName} IsConnectedAndReady WaitFor {timeoutMS}ms FAIL");
        return false;
    }

    public bool IsOnlineRoom()
    {
        if (PhotonNetwork.OfflineMode)
            return false;

        return PhotonNetwork.InRoom;
    }

    public bool IsOfflineRoom()
    {
        if (PhotonNetwork.OfflineMode && PhotonNetwork.InRoom)
            return true;

        return false;
    }
    #endregion
    #region Getter
    public bool TryGetCurrentRoomeName(out string rooName)
    {
        if (!PhotonNetwork.InRoom)
        {
            rooName = "UnKnOwN";
            return false;
        }

        rooName =  PhotonNetwork.CurrentRoom.Name;
        return true;
    }
    #endregion
    #region Setter
    #endregion
    #endregion
}