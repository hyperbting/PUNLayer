public class SerializeReadWrite
{
    public string name;

    //Read from local
    public System.Func<object> Read;
    //Write to local
    public System.Action<object> Write;

    public SerializeReadWrite(System.Func<object> read, System.Action<object> write)
    {
        Read = read;
        Write = write;
    }
}