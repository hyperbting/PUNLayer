using System;
using System.Collections.Generic;

public interface ISerializableHelper
{
    void Register(params SerializableReadWrite[] srw);
    void Unregister(params SerializableReadWrite[] srw);
}