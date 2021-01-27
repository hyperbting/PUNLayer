using System;

public interface IObjectSupplier
{
    void RegisterBuilder(Func<string, string, object> builder);
    void UnregisterBuilder(Func<string, string, object> builder);

    object BuildObject(string objectName, string uuid);
}