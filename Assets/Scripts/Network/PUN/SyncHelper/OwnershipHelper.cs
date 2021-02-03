using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OwnershipHelper : MonoBehaviourPunCallbacks
{
    readonly string scr = "OwnershipHelper";
    public static OwnershipHelper instance;

    public void RequestOwnership(object targetObj)
    {

        if (targetObj == null)
        {
            Debug.Log($"targetObj missing");
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            Debug.Log($"NotInRoom");
            return;
        }

        var scr = (targetObj as GameObject).GetComponent<OwnershipSubAdditive>();
        if (scr == null)
        {
            Debug.Log($"targetObj OwnershipSubAdditive missing");
            return;
        }

        _ = scr.RequestOwnership(PhotonNetwork.LocalPlayer);
    }

    public void ReleaseOwnership(object targetObj)
    {
        if (targetObj == null)
        {
            Debug.Log($"targetObj missing");
            return;
        }

        var scr = (targetObj as GameObject).GetComponent<OwnershipSubAdditive>();
        if (scr == null)
        {
            Debug.Log($"targetObj OwnershipSubAdditive missing");
            return;
        }

        _ = scr.ReleaseOwnership();
    }
}
