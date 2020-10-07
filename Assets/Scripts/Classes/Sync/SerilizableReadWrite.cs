using System;
[Serializable]
public class SerializableReadWrite : SerializableWrite
{
    //Read from local
    public Func<object> Read;

    public SerializableReadWrite (string name, Func<object> read, Action<object> write) : base(name,  write)
    {
        Read = read;
    }

    public override string ToString()
    {
        return string.Format($"{base.ToString()},Read");
    }
}

[System.Serializable]
public class SerializableWrite
{
    public string name;
    //Write to local
    public Action<object> Write;

    public SerializableWrite(Action<object> write)
    {
        Write = write;
    }

    public SerializableWrite(string name, Action<object> write): this(write)
    {
        this.name = name;
    }

    public override string ToString()
    {
        return string.Format($"Name:{name},Write");
    }
}