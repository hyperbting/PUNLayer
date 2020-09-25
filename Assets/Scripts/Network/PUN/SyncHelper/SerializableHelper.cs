using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class SerializableHelper : BaseSyncHelper, IPunObservable, ISerializableHelper
{

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //}

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

