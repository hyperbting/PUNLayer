using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
/// State for Player is implemented by PlayerProperties with OnPlayerPropertiesUpdate() and PhotonNetwork.LocalPlayer.SetCustomProperties()
/// State for Item is implemented by RoomProperties with OnRoomPropertiesUpdate() and PhotonNetwork.CurrentRoom.SetCustomProperties()
public class StateHelper : BaseSyncHelper
{
    #region Player/ Player owned
    public void UpdatePlayerProperties(string key)
    {
        // only update for Owner
        if (photonView.Owner != PhotonNetwork.LocalPlayer)
            return;

        if (dataToSync.TryGetValue(key, out SerializableReadWrite srw))
        {
            UpdatePlayerProperties(key, srw.Read());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        // only apply to local for PhotonView Owner
        if (targetPlayer != photonView.Owner)
            return;

        foreach (var key in changedProps.Keys)
        {
            if (dataToSync.TryGetValue(key.ToString(), out SerializableReadWrite srw))
            {
                srw.Write(changedProps[key]);
            }
        }
    }
    #endregion

    public async Task<bool> UpdateRoomProperties(string key)
    {
        if (dataToSync.TryGetValue(key, out SerializableReadWrite srw))
        {
            return await UpdateRoomProperties(key, srw.Read());
        }

        return false;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnRoomPropertiesUpdate(changedProps);

        //Who should NOT react to this ?

        // apply every state to local
        foreach (var key in changedProps.Keys)
        {
            if (dataToSync.TryGetValue(key.ToString(), out SerializableReadWrite srw))
            {
                srw.Write(changedProps[key]);
            }
        }
    }
}
