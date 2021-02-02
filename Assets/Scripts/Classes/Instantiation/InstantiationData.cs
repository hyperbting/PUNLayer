using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class InstantiationData : Dictionary<string, object>
{
    public SyncTokenType tokenType;

    #region
    public InstantiationData() : base()
    {
    }

    public InstantiationData(object[] data)
    {
        tokenType = (SyncTokenType)data[0];

        //(i, i+1), i=1  
        for (int i = 1; i < data.Length; i += 2)
            this[(string)data[i]] = data[i + 1];
    }

    public object[] ToData()
    {
        var dataList = new List<object>() { tokenType };

        foreach (var kv in this)
        {
            dataList.Add(kv.Key);
            dataList.Add(kv.Value);
        }

        return dataList.ToArray();
    }
    #endregion

    public static InstantiationData Build(SyncTokenType tokenType)
    {
        return new InstantiationData
        {
            tokenType = tokenType
        };
    }

    public static InstantiationData Build(string str)
    {
        return JsonConvert.DeserializeObject<InstantiationData>(str); ;
    }

    public bool TryGetValue(InstantiationKey enumKey, out object value)
    {
        return TryGetValue(enumKey.ToString(), out value);
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public enum InstantiationKey
    {
        none,

        objectuuid,
        objectname,
    }
}