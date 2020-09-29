using System;
using System.Collections.Generic;

public interface ISerializableHelper
{
    void Register(SerilizableReadWrite srw);
    void Unregister(SerilizableReadWrite srw);
}

public interface ISerializeData
{
    //List<string> GetKeys();
    //object GetObjectValue(string key);

    List<KeyValuePair<string, object>> BuildSyncData(string keyPrefix = "");
}