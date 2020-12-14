using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Text;

public class SerializeViewBrust : MonoBehaviour, IPunObservable
{
    [SerializeField] Text randomText;

    StringBuilder sb = new StringBuilder();
    private void Start()
    {
        UpdateSB(RPSetting.burstAmount);
    }

    System.Random rnd = new System.Random();
    char GenerateRandomCharacter()
    {
        return (char)rnd.Next('a', 'z');
    }

    string UpdateSB(int targetAmount)
    {
        while(sb.Length >= targetAmount)
            sb.Remove(0, 1);

        while (sb.Length < targetAmount)
            sb.Append(GenerateRandomCharacter());

        return sb.ToString();
    }

    string newString = "";
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!RPSetting.burst)
        {
            if (randomText != null)
                randomText.text = "";
            return;
        }

        if (stream.IsWriting)
        {
            newString = UpdateSB(RPSetting.burstAmount);
            randomText.text = newString;
            stream.SendNext(newString);
        }
        else
        {
            newString = (string)stream.ReceiveNext();

            if (randomText != null)
                randomText.text = newString;
        }
    }

}
