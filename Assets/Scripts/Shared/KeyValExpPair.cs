using System;

[Serializable]
public class KeyValExpPair : KeyValPair
{
    public object exp;

    public KeyValExpPair(string pairKey, object tarValue = null, object expectValue = null)
    {
        key = pairKey;
        value = tarValue;
        exp = expectValue;
    }

    public void InsertInto(ref ExitGames.Client.Photon.Hashtable valHT, ref ExitGames.Client.Photon.Hashtable expHT)
    {
        valHT[key] = value;

        if (exp == null)
            expHT[key] = null;
        else
            expHT[key] = exp;
    }

    public override string ToString()
    {
        return string.Format($"K:{key}, From {PrintObject(exp)} to {PrintObject(value)}");
    }
}

[Serializable]
public class KeyValPair
{
    public string key;

    public object value;

    protected string PrintObject(object obj)
    {
        if (obj == null)
            return "null";

        return obj.ToString();
    }

    public override string ToString()
    {
        return string.Format($"K:{key} V:{PrintObject(value)}");
    }
}