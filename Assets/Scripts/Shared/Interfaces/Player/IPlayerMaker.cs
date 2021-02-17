using UnityEngine;

public interface IObjectMaker
{
    GameObject GetMine();

    GameObject InstantiateObject();
    void DestroyObject();

    //GameObject InstantiateRemoteObject(string uuid, Transform parent);
}
