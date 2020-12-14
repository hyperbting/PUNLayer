using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Text;

public class SerializeViewBrust : MonoBehaviour, IPunObservable
{

    StringBuilder sb = new StringBuilder();

    [SerializeField] Text randomText;
    string newString = "";
    string UIText
    {
        set
        {
            if (newString == value)
                return;

            newString = value;
            randomText.text = newString;
        }
        get
        {
            return newString;
        }
    }

    private void Start()
    {
        UpdateSB(RPSetting.burstAmount);
    }

    void Murmur()
    {
        UIText = UpdateSB(RPSetting.burstAmount);
    }

    public void LateUpdate()
    {
        if (!RPSetting.burst)
        {
            UIText = "";
            return;
        }

        Murmur();
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!RPSetting.burst)
        {
            return;
        }

        if (stream.IsWriting)
        {
            stream.SendNext(UIText);
        }
        else
        {
            UIText = (string)stream.ReceiveNext();
        }
    }

    #region support
    System.Random rnd = new System.Random();
    char GenerateRandomCharacter()
    {
        return (char)rnd.Next('a', 'z');
    }

    string UpdateSB(int targetAmount)
    {
        while (sb.Length >= targetAmount)
            sb.Remove(0, 1);

        while (sb.Length < targetAmount)
            sb.Append(GenerateRandomCharacter());

        return sb.ToString();
    }
    #endregion
}
