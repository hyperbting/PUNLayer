﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : SingletonMonoBehaviour<ServiceManager>
{
    public NetworkSystem networkSystem;
    public PlayerMaker playerMaker;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
