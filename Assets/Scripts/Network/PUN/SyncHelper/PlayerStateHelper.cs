using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
/// State for Player is implemented by PlayerProperties with OnPlayerPropertiesUpdate() and PhotonNetwork.LocalPlayer.SetCustomProperties()
public class PlayerStateHelper : BaseSyncHelper
{
    #region Player/ Player owned
    public void UpdatePlayerProperties(string key)
    {
        // only update for Owner
        if (photonView.Owner != PhotonNetwork.LocalPlayer)
            return;

        if (dataToSync.TryGetValue(key, out SerializableReadWrite srw))
        {
            UpdatePlayerProperties(key, srw.Read());//UpdatePlayerProperties(key, srw.Read[0]());
        }
    }
    #endregion
}
