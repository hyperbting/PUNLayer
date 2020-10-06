[System.Serializable]
public class OnChangedReadWrite : SerializableWrite
{
    //Read from local
    public System.Func<object> Read;

    public OnChangedReadWrite(string name, System.Func<object> read, System.Action<object> write) : base(name, write)
    {
        Read = read;
    }
}

/// <summary>
/// Remote side, receive data from server, and dealing with UpdateToLocal only.
/// </summary>
[System.Serializable]
public class OnChangedWrite
{
    public string name;

    //Write to local
    public System.Action<object> Write;

    public OnChangedWrite(System.Action<object> write)
    {
        Write = write;
    }

    public OnChangedWrite(string name, System.Action<object> write) : this(write)
    {
        this.name = name;
    }
}
