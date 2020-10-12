using System;
using System.Collections.Generic;

[Serializable]
public class InstantiationData: Dictionary<string,string>
{
    public SyncTokenType tokenType;
    public List<string> keyValuePairs = new List<string>();

    #region
    public InstantiationData():base()
    {
    }

    public InstantiationData(object[] data)
    {
        tokenType = (SyncTokenType)data[0];
        keyValuePairs = (List<string>)data[1];
        list2Dic();
    }

    public object[] ToData()
    {
        var dataList = new List<object>() { tokenType };

        dic2List();
        dataList.Add(keyValuePairs);

        return dataList.ToArray();
    }
    #endregion

    public static InstantiationData Build(string str)
    {
        var newone = UnityEngine.JsonUtility.FromJson<InstantiationData>(str);
        newone.list2Dic();
        return newone;
    }

    public override string ToString()
    {
        dic2List();
        return UnityEngine.JsonUtility.ToJson(this);
    }

    #region inner helper
    void dic2List()
    {
        // pull from dictionary
        keyValuePairs.Clear();

        foreach (var kv in this)
        {
            keyValuePairs.Add(kv.Key);
            keyValuePairs.Add(kv.Value);
        }
    }

    void list2Dic()
    {
        if (keyValuePairs == null  || keyValuePairs.Count <= 0)
            return;

        // put into dictionary
        for (int i = 0; i < keyValuePairs.Count; i+=2)
        {
            this[keyValuePairs[i]] = keyValuePairs[i+1];
        }

        keyValuePairs.Clear();
    }
    #endregion
}