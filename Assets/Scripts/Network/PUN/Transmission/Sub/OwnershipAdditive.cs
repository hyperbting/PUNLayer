using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OwnershipAdditive : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{

    ITransmissionBase itb;

    [SerializeField] OwnershipOption ownershipOption = OwnershipOption.Request;

    #region Interface 
    public Action<Player> ownershipRequestEvent;

    public Action<Player,Player> ownershipTransferedEvent;
    #endregion

    private void Awake()
    {
        switch (photonView.OwnershipTransfer)
        {
            case OwnershipOption.Fixed:
                Debug.LogWarning($"Improper OwnershipOption, now set to {ownershipOption}");

                //must set to proper default
                photonView.OwnershipTransfer = ownershipOption;
                break;
            default:
                //safe to use Transfer
                break;
        }
    }

    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        this.itb = itb;
    }

    #region
    TaskCompletionSource<bool> tcs;
    public async Task<bool> RequestOwnership(Player newOwner)
    {
        switch (photonView.OwnershipTransfer)
        {
            case OwnershipOption.Request:
                photonView.RequestOwnership();

                tcs = new TaskCompletionSource<bool>();
                await Task.WhenAny(tcs.Task, Task.Delay(10000));
                tcs.TrySetResult(false);

                return await tcs.Task;
            case OwnershipOption.Takeover:
                photonView.TransferOwnership(newOwner);
                return true;
            default:
            case OwnershipOption.Fixed:
                break;
        }

        return false;
    }
    #endregion

    #region
    void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != photonView)
            return;

        ownershipRequestEvent?.Invoke(requestingPlayer);
    }

    void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != photonView)
            return;

        tcs?.TrySetResult(true);

        ownershipTransferedEvent?.Invoke(previousOwner, targetView.Owner);
    }
    #endregion
}
