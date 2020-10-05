using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour, ITokenHandler
{
    public NetworkSystem parentSystem {
        get
        {
            return ServiceManager.Instance.networkSystem;
        }
    }

    public PlayerTransmission transmissionToken;
    Transform transmissionTransform;

    private void OnEnable()
    {
        //Register for Token with NetworkSystem
        parentSystem.OnJoinedRoomEvent += OnJoinedRoomAct;
    }

    private void OnDisable()
    {
        parentSystem.OnJoinedRoomEvent -= OnJoinedRoomAct;
    }

    public void Start()
    {
    }

    public void OnJoinedRoomAct()
    {
        Debug.Log($"OnJoinedRoomAct");

        var hostPlayerGO = PlayerManager.Instance.GetHostPlayer();
        var nt = parentSystem.RequestSyncToken(hostPlayerGO.transform);
        transmissionToken = nt.GetComponent<PlayerTransmission>();

        hostPlayerGO.GetComponent<Player>().RegisterWithTransmissionToken(transmissionToken);
    }
}

public interface ITokenHandler
{

}