
using System.ComponentModel;
using UnityEngine;
using UnityWeld.Binding;

[Binding]
public class PUNConnectorDebugerVM : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    #region
    private string roomName;
    [Binding]
    public string RoomName
    {
        get
        {
            return roomName;
        }
        set
        {
            if (roomName == value)
            {
                return; // No change.
            }

            roomName = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoomName"));//OnRoomNamePropertyChanged("RoomName");
        }
    }

    private string rpKeyText;
    [Binding]
    public string RPKeyText
    {
        get
        {
            return rpKeyText;
        }
        set
        {
            if (rpKeyText == value)
            {
                return; // No change.
            }

            rpKeyText = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RPKeyText"));
        }
    }

    private string rpNewText;
    [Binding]
    public string RPNewText
    {
        get
        {
            return rpNewText;
        }
        set
        {
            if (rpNewText == value)
            {
                return; // No change.
            }

            rpNewText = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RPNewText"));
        }
    }

    private string rpOriginText;
    [Binding]
    public string RPOriginText
    {
        get
        {
            return rpOriginText;
        }
        set
        {
            if (rpOriginText == value)
            {
                return; // No change.
            }

            rpOriginText = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RPOriginText"));
        }
    }
    #endregion

    [Binding]
    public void Connect()
    {
        _ = PUNConnecter.Instance.ConnectToServer();
    }

    [Binding]
    public void OfflineMode()
    {
        _ = PUNConnecter.Instance.Disconnect();
    }

    [Binding]
    public void JoinRoom()
    {
        _ = PUNConnecter.Instance.JoinGameRoom(RoomName);
    }

    [Binding]
    public void LeaveRoom()
    {
        _ = PUNConnecter.Instance.LeaveRoom();
    }

    [Binding]
    public void TrySetRoomProperties()
    {
        if (string.IsNullOrEmpty(rpKeyText))
        {
            return;
        }

        KeyValExpPair kvr = new KeyValExpPair(rpKeyText);

        if (!string.IsNullOrEmpty(rpNewText))
        {
            kvr.value = rpNewText;
        }

        if (!string.IsNullOrEmpty(rpOriginText))
        {
            kvr.exp = rpOriginText;
        }

        _ = PUNConnecter.Instance.SetRoomProperty(kvr);
    }

    [Binding]
    public void TrySetPlayerProperties()
    {
        if (string.IsNullOrEmpty(rpKeyText))
        {
            return;
        }

        KeyValExpPair kvr = new KeyValExpPair(rpKeyText);

        if (!string.IsNullOrEmpty(rpNewText))
        {
            kvr.value = rpNewText;
        }

        if (!string.IsNullOrEmpty(rpOriginText))
        {
            kvr.exp = rpOriginText;
        }

        _ = PUNConnecter.Instance.SetPlayerProperty(Photon.Pun.PhotonNetwork.LocalPlayer, kvr);
    }

    [Binding]
    public void UpdateRoomList()
    {
        _ = PUNConnecter.Instance.UpdateRoomList();
    }

    [Binding]
    public void FetchRegionList()
    {
        _ = PUNConnecter.Instance.FetchRegionList();
    }
}
