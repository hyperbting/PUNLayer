using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
/// Serializable for Player is implemented by OnPhotonSerializeView
public class SerializableHelper : BaseSyncHelper, IPunObservable, ISerializableHelper
{
    /// <summary>
    /// To Store method to read/ write specific value
    /// </summary>
    protected new Dictionary<string, SerializableReadWrite> dataToSync = new Dictionary<string, SerializableReadWrite>();

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
                if (dataToSync.TryGetValue(keys[i], out SerializableReadWrite val))
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
                if (dataToSync.TryGetValue(keys[i], out SerializableReadWrite  val))
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

