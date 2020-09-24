using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PUNConnectorDebuger : MonoBehaviour
{
    public PUNConnecter inc;

    public InputField roomNameInput;

    public void LeaveRoom()
    {
        _ = inc.LeaveRoom();
    }

    public void UpdateRoomList()
    {
        _ = inc.UpdateRoomList();
    }

    public void FetchRegionList()
    {
        inc.FetchRegionList();
    }

    //public void UpdateRegionList()
    //{
    //    inc.UpdateRegionPing();
    //}

    #region Room
    public Text rpKeyText;
    public Text rpNewText;
    public Text rpOriginText;
    public void TrySetRoomProperties()
    {
        if (string.IsNullOrEmpty(rpKeyText.text))
        {
            return;
        }

        KeyValExpPair kvr = new KeyValExpPair(rpKeyText.text);

        if (!string.IsNullOrEmpty(rpNewText.text))
        {
            kvr.value = rpNewText.text;
        }

        if (!string.IsNullOrEmpty(rpOriginText.text))
        {
            kvr.exp = rpOriginText.text;
        }

        _ = inc.SetRoomProperty(kvr);
    }

    public void TrySetPlayerProperties()
    {
        if (string.IsNullOrEmpty(rpKeyText.text))
        {
            return;
        }

        KeyValExpPair kvr = new KeyValExpPair(rpKeyText.text);

        if (!string.IsNullOrEmpty(rpNewText.text))
        {
            kvr.value = rpNewText.text;
        }

        if (!string.IsNullOrEmpty(rpOriginText.text))
        {
            kvr.exp = rpOriginText.text;
        }

        _ = inc.SetPlayerProperty(Photon.Pun.PhotonNetwork.LocalPlayer, kvr);
    }
    #endregion
}
