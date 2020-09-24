using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///This script Register those ITEMs belong to Player but not Scene
///These items will also be gone when owner left
public class PersonalItem : MonoBehaviour
{
    //PlayerTransmission pm;
    [SerializeField]
    string itemName;
    [SerializeField]
    List<SyncAttribute> syncTargets = new List<SyncAttribute>();

    public void Setup(string itemName)
    {
        this.name = "pu(" + itemName;
        this.itemName = itemName;
    }

    #region Sync using PlayerTransmissionToken
    public void BuildSyncData()
    {

    }
    #endregion
}

public struct SyncAttribute
{
    public string name;
    public Action<object> Read;
    public Func<object> Write;

    public Action OnValueChanged;
}

