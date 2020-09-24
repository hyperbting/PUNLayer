using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class SerializableHelper : MonoBehaviour, IPunObservable, ISerializableHelper
{
    Dictionary<string, SerilizableReadWrite> dataToSync = new Dictionary<string, SerilizableReadWrite>();

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //}

    #region interface
    public void Register(SerilizableReadWrite srw)
    {
        if (!dataToSync.ContainsKey(srw.name))
        {
            dataToSync.Add(srw.name, srw);
            Debug.Log($"{srw.name} Registered");
        }
    }

    public void Unregister(SerilizableReadWrite srw)
    {
        Unregister(srw.name);
    }

    public void Unregister(string key)
    {
        if (!dataToSync.ContainsKey(key))
            return;

            Debug.Log($"{key} Unregistered");
            dataToSync.Remove(key);
    }
    #endregion

    #region Photon Callback
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var keys = new List<string>(dataToSync.Keys);
        if (stream.IsWriting)
        {
            //Debug.Log($"IsWriting");
            for (int i = 0; i < keys.Count; i++)
            {
                //Debug.Log($"TryGetValue for Key:{keys[i]}");
                if (dataToSync.TryGetValue(keys[i], out SerilizableReadWrite val))
                {
                    var va = val?.Read();
                    //Debug.Log($" Key:{keys[i]} {va}");
                    stream.SendNext(va);
                }
            }
        }
        else
        {
            //Debug.Log($"IsReading");
            for (int i = 0; i < keys.Count; i++)
            {
                //Debug.Log($"TryGetValue for Key:{keys[i]}");
                if (dataToSync.TryGetValue(keys[i], out SerilizableReadWrite val))
                {
                    var va = stream.ReceiveNext();
                    //Debug.Log($"{va} Received");
                    val?.Write(va);
                }
            }
        }
    }
    #endregion
}

