using System;
using System.Collections.Generic;

public interface ISerializableHelper
{
    void Register(params SerializableReadWrite[] srw);
    void Unregister(params SerializableReadWrite[] srw);
}

public interface ISerializeData
{
    //List<string> GetKeys();
    //object GetObjectValue(string key);

    List<KeyValuePair<string, object>> BuildSyncData(string keyPrefix = "");
}