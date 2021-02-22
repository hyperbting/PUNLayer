using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SingletonMonoBehaviour<ObjectManager>, IObjectSupplyManager
{
    Func<string, string, object> ObjectBuilder;
    Action<string, string> ObjectDestroyer;

    #region IObjectSupplier
    public void RegisterObjectSupplier(IObjectSupplier objSupplier)
    {
        ObjectBuilder += objSupplier.BuildLocalObject;
        ObjectDestroyer += objSupplier.DestroyLocalObject;
    }

    public void UnregisterObjectSupplier(IObjectSupplier objSupplier)
    {
        ObjectBuilder -= objSupplier.BuildLocalObject;
        ObjectDestroyer -= objSupplier.DestroyLocalObject;
    }


    public object BuildObject(string objectName, string uuid)
    {
        if (ObjectBuilder == null)
            return null;

        object obj = null;
        foreach (var fa in ObjectBuilder.GetInvocationList())
        {
            obj = fa.DynamicInvoke(objectName, uuid);
            if (obj != null)
                break;
        }

        return obj;
    }

    public void DestroyObject(string objName, string UUID)
    {
        if (ObjectDestroyer == null)
            return;

        ObjectDestroyer.Invoke(objName, UUID);
    }

    #endregion
}
