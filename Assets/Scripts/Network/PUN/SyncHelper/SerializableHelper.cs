using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
/// Serializable for Player is implemented by OnPhotonSerializeView
public class SerializableHelper : BaseSyncHelper, IPunObservable
{
    #region Photon Callback
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var keys = new List<string>(dataToSync.Keys);
        if (stream.IsWriting)
        {
            SendToRemote(keys, stream);
        }
        else
        {
            ReadToLocal(keys, stream);
        }
    }
    #endregion

    void SendToRemote(List<string> keys, PhotonStream stream)
    {
        //Debug.Log($"IsWriting {keys.Count}");
        for (int i = 0; i < keys.Count; i++)
        {
            //Debug.Log($"TryGetValue for Key:{keys[i]}");
            if (dataToSync.TryGetValue(keys[i], out SerializableReadWrite val))
            {
                for (int j = 0; j < val.Read.Length; j++)
                {
                    var va = val?.Read[j]();
                    //Debug.Log($" Key:{keys[i]}-{j} {va}");
                    stream.SendNext(va);
                }
            }
        }
    }

    void ReadToLocal(List<string> keys, PhotonStream stream)
    {
        //Debug.Log($"IsReading {keys.Count}");
        for (int i = 0; i < keys.Count; i++)
        {
            //Debug.Log($"TryGetValue for Key:{keys[i]}");
            if (dataToSync.TryGetValue(keys[i], out SerializableReadWrite val))
            {
                for (int j = 0; j < val.Write.Length; j++)
                {
                    var va = stream.ReceiveNext();
                    //Debug.Log($"{va}-{j} Received");
                    val?.Write[j](va);
                }
            }
        }
    }
}

