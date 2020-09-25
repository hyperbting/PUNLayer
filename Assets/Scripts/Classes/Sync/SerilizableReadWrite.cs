[System.Serializable]
public class SerilizableReadWrite: SerilizableWrite
{
    //Read from local
    public System.Func<object> Read;

    public SerilizableReadWrite(System.Func<object> read, System.Action<object> write): base(write)
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
}