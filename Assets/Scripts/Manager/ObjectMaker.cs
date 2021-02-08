using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaker : MonoBehaviour
{
    private void OnEnable()
    {
        ObjectManager.Instance.RegisterBuilder(BuildLocalGameObject);
    }

    private void OnDisable()
    {
        ObjectManager.Instance.UnregisterBuilder(BuildLocalGameObject);
    }
    public GameObject roomobject;

    Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
    public GameObject BuildLocalGameObject(string objName, string UUID = null)
    {
        Debug.LogWarning("[ObjectMaker] BuildLocalGameObject");

        GameObject go = null;
        switch (objName)
        {
            case "RoomBaseObject":
                Debug.Log("RoomBaseObject");

                //LookUp before Create
                if (UUID != null && dic.TryGetValue(UUID, out go))
                {
                    return go;
                }

                go = Instantiate(roomobject, transform);
                dic[UUID] = go;

                break;
        }

        return go;
    }
}
