using System;

public interface IObjectSupplyManager
{
    void RegisterObjectSupplier(IObjectSupplier objSupplier);
    void UnregisterObjectSupplier(IObjectSupplier objSupplier);

    object BuildObject(string objectName, string uuid);
    void DestroyObject(string objectName, string uuid);
}