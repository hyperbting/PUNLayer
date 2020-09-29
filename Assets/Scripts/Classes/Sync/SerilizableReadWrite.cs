[System.Serializable]
public class SerilizableReadWrite: SerilizableWrite
{
    //Read from local
    public System.Func<object> Read;

    public SerilizableReadWrite(string name, System.Func<object> read, System.Action<object> write) : base(name,  write)
    {
        Read = read;
    }
}

[System.Serializable]
public class SerilizableWrite
{
    public string name;
    //Write to local
    public System.Action<object> Write;

    public SerilizableWrite( System.Action<object> write)
    {
        Write = write;
    }

    public SerilizableWrite(string name, System.Action<object> write): this(write)
    {
        this.name = name;
    }
}