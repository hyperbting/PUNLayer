using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///This script Register those ITEMs belong to Player but not Scene
///These items will also be gone when owner left
public class PersonalItem : MonoBehaviour, ISerializeData
{
    // one or more data to sync for single item
    List<SerilizableReadWrite> srw = new List<SerilizableReadWrite>();
    //PlayerTransmission pm;
    [SerializeField]
    string itemName;

    public void Setup(string itemName)
    {
        this.name = "pu(" + itemName;
        this.itemName = itemName;

        srw.Add(new SerilizableReadWrite("Name", ()=> { return itemName; }, (object value)=> { itemName = (string)value; }));
    }

    #region ISerializeData
    public List<KeyValuePair<string, object>> BuildSyncData(string keyPrefix = "pi")
    {
        var res = new List<KeyValuePair<string, object>>();
        foreach (var data in srw)
        {
            res.Add(new KeyValuePair<string,object>(data.name, data.Read()));
        }
        return res;
    }
    #endregion
}

