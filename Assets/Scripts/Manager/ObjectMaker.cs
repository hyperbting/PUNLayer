using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaker : MonoBehaviour, IObjectSupplier
{
    private void OnEnable()
    {
        ObjectManager.Instance.RegisterObjectSupplier(this);
    }

    private void OnDisable()
    {
        ObjectManager.Instance.UnregisterObjectSupplier(this);
    }

    public GameObject roomobjectPrefab;

    Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
    public object BuildLocalObject(string objName, string UUID)
    {
        Debug.LogWarning("[ObjectMaker] BuildLocalObject");

        GameObject go = null;
        switch (objName)
        {
            case "RoomObject":
                Debug.Log("Create RoomObject");

                //LookUp before Create
                if (UUID != null && dic.TryGetValue(UUID, out go))
                {
                    return go;
                }

                go = Instantiate(roomobjectPrefab);
                dic[UUID] = go;

                break;
        }

        return go;
    }

    public void DestroyLocalObject(string objName, string UUID)
    {
        Debug.LogWarning("[ObjectMaker] DestroyLocalObject");

        GameObject go = null;
        switch (objName)
        {
            case "RoomBaseObject":
                Debug.Log("TryDestroy RoomBaseObject");

                //LookUp
                if (dic.TryGetValue(UUID, out go))
                {
                    GameObject.Destroy(go);
                    dic.Remove(UUID);
                    return;
                }
                break;
        }
    }
}
