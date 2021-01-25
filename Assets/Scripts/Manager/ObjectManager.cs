using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SingletonMonoBehaviour<ObjectManager>, IObjectSupplier
{
    public Func<string, string, object> ObjectBuilder
    {
        get;
        set;
    }

    public void RegisterBuilder(Func<string, string, object> builder)
    {
        ObjectBuilder += builder;
    }

    public void UnregisterBuilder(Func<string, string, object> builder)
    {
        ObjectBuilder -= builder;
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

}
