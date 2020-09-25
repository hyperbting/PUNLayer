using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class StateHelper : BaseSyncHelper
{
    #region
    public void UpdatePlayerProperties(string key)
    {
        // only update for Owner
        if (photonView.Owner != PhotonNetwork.LocalPlayer)
            return;

        if (dataToSync.TryGetValue((string)key, out SerilizableReadWrite srw))
        {
            ht.Clear();
            ht.Add((string)key, srw.Read());

            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
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
            if (dataToSync.TryGetValue((string)key, out SerilizableReadWrite srw))
            {
                srw.Write(changedProps[key]);
            }
        }
    }
    #endregion
}
