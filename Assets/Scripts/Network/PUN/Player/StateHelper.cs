using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHelper : MonoBehaviourPunCallbacks
{
    #region
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        // only update for self
        if (targetPlayer != photonView.Owner)
            return;

        foreach (var key in changedProps.Keys)
        {
            //if (personalStates.TryGetValue((string)key, out GameObject go))
            //{

            //}
        }
    }
    #endregion
}
