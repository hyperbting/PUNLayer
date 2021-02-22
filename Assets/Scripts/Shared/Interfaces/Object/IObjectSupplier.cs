public interface IObjectSupplier
{
    object BuildLocalObject(string objName, string UUID);
    void DestroyLocalObject(string objName, string UUID);
}