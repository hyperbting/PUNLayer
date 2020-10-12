using System;
using System.Collections.Generic;

[Serializable]
public class InstantiationData
{
    public SyncTokenType tokenType;
    public List<KVP> keyValuePairs = new List<KVP>();

    Dictionary<string, string> dic = new Dictionary<string, string>();
    #region
    public InstantiationData(object[] data)
    {
        tokenType = (SyncTokenType)data[0];
        keyValuePairs = (List<KVP>)data[1];
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

        if (dic == null || dic.Count <= 0)
            return;

        foreach (var kv in dic)
        {
            keyValuePairs.Add(new KVP(kv.Key, kv.Value));
        }
    }

    void list2Dic()
    {
        dic = new Dictionary<string, string>();

        if (keyValuePairs == null  || keyValuePairs.Count <= 0)
            return;

        // put into dictionary
        foreach (var kv in keyValuePairs)
        {
            dic[kv.Key] = kv.Val;
        }

        keyValuePairs.Clear();
    }
    #endregion
}

[Serializable]
public class KVP
{
    public string Key;
    public string Val;

    public KVP(string key, string value)
    {
        Key = key;
        Val = value;
    }

    public static KVP Build(string whole)
    {
        return UnityEngine.JsonUtility.FromJson<KVP>(whole);
    }

    public override string ToString()
    {
        return UnityEngine.JsonUtility.ToJson(this);
    }
}