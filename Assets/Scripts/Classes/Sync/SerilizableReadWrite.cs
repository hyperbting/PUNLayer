using System;

public class SerializableReadWrite : SerializableWrite
{
    public new SyncHelperType syncType = SyncHelperType.Serializable;

    //Read from local
    public Func<object>[] Read;

    public SerializableReadWrite (string name, Func<object> readFromLocal, Action<object> writeWhenPropUpdate) : base(name, writeWhenPropUpdate)
    {
        Read = new Func<object>[1] { readFromLocal };
    }

    public SerializableReadWrite(string name, Func<object>[] readFromLocal, Action<object>[] writeWhenPropUpdate) : base(name, writeWhenPropUpdate)
    {
        if (readFromLocal.Length != writeWhenPropUpdate.Length)
        {
            //TODO: Warn
        }

        Read = readFromLocal;
    }

    public override string ToString()
    {
        return string.Format($"{base.ToString()},Read");
    }
}

public class SerializableWrite
{
    public SyncHelperType syncType = SyncHelperType.RoomState;

    public string name;
    //Write to local
    public Action<object>[] Write;

    public SerializableWrite(Action<object> write)
    {
        Write = new Action<object>[1] { write };
    }

    public SerializableWrite(Action<object>[] writes)
    {
        Write = writes;
    }

    public SerializableWrite(string name, Action<object> write): this(write)
    {
        this.name = name;
    }

    public SerializableWrite(string name, Action<object>[] writes) : this(writes)
    {
        this.name = name;
    }

    public override string ToString()
    {
        return string.Format($"Name:{name},Write");
    }
}

public enum SyncHelperType
{
    None,
    PlayerState,
    RoomState,
    Serializable
}
