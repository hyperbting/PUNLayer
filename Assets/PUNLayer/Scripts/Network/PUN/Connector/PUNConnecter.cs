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
    [SerializeField] ServerSettings serSettings;
    public ServerSettings SerSettings
    {
        get
        {
            if (serSettings != null)
                return serSettings;

            serSettings = Resources.Load<ServerSettings>("PhotonServerSettings");
                
            Debug.LogWarning("ServerSettings Missing! TryLoad From Resources");
            return serSettings;
        }
    }

    #region CurrentPhotonRoomState, OnPhotonRoomStateChange
    Action<PhotonRoomState, PhotonRoomState> OnPhotonRoomStateChange;
    [SerializeField]
    PhotonRoomState currentPhotonRoomState = PhotonRoomState.Disconnected;
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

    [SerializeField] ServerTarget serMasterTarget;

    #endregion Init
    public void Init(INetworkConnectUser incUser)
    {
        incu = incUser;
    }

    #region PUN OnCallbackAction
    public Action OnJoinedRoomAction;
    public Action OnLeftRoomAction;
    public Action<short, string> OnCreateRoomFailedAction;
    public Action<short, string> OnJoinRoomFailedAction;
    public Action<Photon.Realtime.Player> OnPlayerEnteredRoomAction;
    public Action<Photon.Realtime.Player> OnPlayerLeftRoomAction;

    public Action<ExitGames.Client.Photon.EventData> OnEventAction;

    /// <summary>
    /// Key, Value
    /// </summary>
    public Action<string, object> OnRoomPropertyUpdateAction;
    public Action<Photon.Realtime.Player, string, object> OnPlayerPropertyUpdateAction;
    #endregion

    #region Mono
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.OpResponseReceived += NetworkingClientOnOpResponseReceived;

        OnPlayerPropertyUpdateAction += CompareWithPPInProgress;
        OnRoomPropertyUpdateAction += CompareWithRPInProgress;

        OnEventAction += ManualBuildSyncTokenOnEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.OpResponseReceived -= NetworkingClientOnOpResponseReceived;

        OnPlayerPropertyUpdateAction -= CompareWithPPInProgress;
        OnRoomPropertyUpdateAction -= CompareWithRPInProgress;

        OnEventAction -= ManualBuildSyncTokenOnEvent;
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
            {
                Debug.LogWarning($"{scriptName} IsConnectedAndReady {PhotonNetwork.NetworkClientState} in less than {i*100}ms");
                return true;
            }
        }

        Debug.LogWarning($"{scriptName} IsConnectedAndReady WaitFor {timeoutMS}ms TimeOut");
        return false;
    }

    public bool IsInRoom(OnOffline ooline=OnOffline.Any, string requestedRoomName="")
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning($"{scriptName} IsInRoom NotInRoom");
            return false;
        }

        if (!string.IsNullOrWhiteSpace(requestedRoomName) && PhotonNetwork.CurrentRoom.Name != requestedRoomName)
        {
            Debug.LogWarning($"{scriptName} IsInRoom NotInSpecificSoom:{requestedRoomName}");
            return false;
        }

        switch (ooline)
        {
            case OnOffline.Offline:
                if (!PhotonNetwork.OfflineMode)
                {
                    Debug.LogWarning($"{scriptName} IsInRoom NotOffline");
                    return false;
                }
                break;
            case OnOffline.Online:
                if (PhotonNetwork.OfflineMode)
                {
                    Debug.LogWarning($"{scriptName} IsInRoom NotOnline");
                    return false;
                }
                break;
            case OnOffline.Any:
            default:
                break;
        }

        return true;
    }

    public bool IsRoomOwner()
    {
        return PhotonNetwork.IsMasterClient;
    }
    #endregion

    #region Getter
    public int GetNetworkID()
    {
        if (IsInRoom(OnOffline.Any))
            return PhotonNetwork.LocalPlayer.ActorNumber;

        Debug.LogWarning($"GetNetworkID: NotInRoom");
        return -1;
    }

    public bool TryGetCurrentRoomeName(out string rooName)
    {
        if (!IsInRoom(OnOffline.Any))
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