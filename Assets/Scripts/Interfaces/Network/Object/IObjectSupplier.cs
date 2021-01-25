using System;

public interface IObjectSupplier
{
    Func<string, string, object> ObjectBuilder { get; set; }

    void RegisterBuilder(Func<string, string, object> builder);
    void UnregisterBuilder(Func<string, string, object> builder);

    object BuildObject(string objectName, string uuid);
}