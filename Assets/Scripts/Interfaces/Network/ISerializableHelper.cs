using System;
using System.Collections.Generic;

public interface ISerializableHelper
{
    void Register(params SerializableWrite[] srw);
    void Unregister(params SerializableWrite[] srw);
}

public interface ISerializeData
{
    //List<string> GetKeys();
    //object GetObjectValue(string key);

    List<KeyValuePair<string, object>> BuildSyncData(string keyPrefix = "");
}