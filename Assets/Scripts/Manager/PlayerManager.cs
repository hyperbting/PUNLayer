﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IPlayerMaker
{
    public ServiceManager serviceManager;
    public static PlayerManager Instance;
    public GameObject playerCorePref;

    [Header("Debug")]
    [SerializeField]
    Player hostPlayer;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        serviceManager.networkSystem.OnJoinedRoomEvent += OnJoinedRoomAct;
    }

    private void OnDisable()
    {
        serviceManager.networkSystem.OnJoinedRoomEvent -= OnJoinedRoomAct;
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiatePlayerObject();
    }

    public GameObject GetHostPlayer()
    {
        return hostPlayer.gameObject;
    }

    public GameObject InstantiatePlayerObject()
    {
        var go = Instantiate(playerCorePref);
        go.name = "HostPlayer";
        var playerScript = go.GetComponent<Player>();
        if (playerScript != null)
        {
            DontDestroyOnLoad(playerScript);
            hostPlayer = playerScript;
            playerScript.isHost = true;
        }
        return go;
    }

    public GameObject InstantiateRemotePlayerObject(string uuid)
    {
        var go = Instantiate(playerCorePref);
        var playerScript = go.GetComponent<Player>();
        if (playerScript != null)
        {
        }
        return go;
    }

    public void SyncPersonalItems()
    {
    }

    public void OnJoinedRoomAct()
    {
        Debug.Log($"OnJoinedRoomAct");
        //var ptScript = networkedPlayerToken.GetComponent<PlayerTransmission>();
        //hostPlayer.RegisterWithTransmissionToken(ptScript);
    }
}
