[System.Serializable]
public class SerializableReadWrite : SerializableWrite
{
    //Read from local
    public System.Func<object> Read;

    public SerializableReadWrite (string name, System.Func<object> read, System.Action<object> write) : base(name,  write)
    {
        Read = read;
    }
}

[System.Serializable]
public class SerializableWrite
{
    public string name;
    //Write to local
    public System.Action<object> Write;

    public SerializableWrite( System.Action<object> write)
    {
        Write = write;
    }

    public SerializableWrite(string name, System.Action<object> write): this(write)
    {
        this.name = name;
    }
}