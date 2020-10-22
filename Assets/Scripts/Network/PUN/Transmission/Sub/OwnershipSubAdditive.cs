using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OwnershipSubAdditive : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks//, ITokenAdditive
{

    ITransmissionBase parent;

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
        parent = itb;

        if (photonView.Owner == null)
        {
            Debug.Log($"InRoomObject:{photonView.ViewID} OwnedByRoom");
        }
        else if (photonView.IsMine)
        {
            Debug.Log($"I({PhotonNetwork.LocalPlayer.UserId}) Own {photonView.ViewID}" + photonView.Owner.ToStringFull());
        }
        else
        {
            // deal with photonView.InstantiationData
            Debug.Log($"{photonView.ViewID} TryLoadData with data {data.ToString()}");
        }
    }

    #region
    TaskCompletionSource<bool> tcs;
    public async Task<bool> RequestOwnership(Player newOwner)
    {
        Debug.Log($"RequestOwnership: OwnedBy {photonView.OwnerActorNr}");
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Debug.Log($"RequestOwnership: AlreadyControlledByMyself");
            return true;
        }

        switch (photonView.OwnershipTransfer)
        {
            case OwnershipOption.Request:
                photonView.RequestOwnership();

                tcs = new TaskCompletionSource<bool>();
                await Task.WhenAny(tcs.Task, Task.Delay(10000));
                tcs.TrySetResult(false);

                Debug.Log($"RequestOwnership Result:{tcs.Task.Result} {photonView.OwnerActorNr}");
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

    public void ReleaseOwnership()
    {
        if (!photonView.IsMine)
        {
            Debug.LogWarning($"ReleaseOwnership: this owned by {photonView.Owner}");
            return;
        }

        Debug.Log($"ReleaseOwnership: this owned by {photonView.Owner}, IsMine:{photonView.IsMine}, ControlledBy {photonView.Controller}");
        photonView.TransferOwnership(0);
    }
    #endregion

    #region Photon.Pun.IPunOwnershipCallbacks
    void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requester)
    {
        // only deal with the case that targetView is the same as my photonView
        if (targetView != photonView)
            return;

        Debug.Log($"OnOwnershipRequest {targetView.ViewID}: {targetView.ToString()} RequestBy {requester.ToString()} ...");
        ownershipRequestEvent?.Invoke(requester);

        //I am MC, targetViewBelong to scene
        if (PhotonNetwork.IsMasterClient && targetView.Owner == null)
            targetView.TransferOwnership(requester);
    }

    void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        // only deal with the case that targetView is the same as my photonView
        if (targetView != photonView)
            return;

        tcs?.TrySetResult(true);

        ownershipTransferedEvent?.Invoke(previousOwner, targetView.Owner);
        Debug.Log($"OnOwnershipTransfered: {targetView.ToString()} {(previousOwner==null ? "Scene" : previousOwner.ToString())} to {(targetView.Owner == null ? "Scene" : targetView.Owner.ToString())}");
    }
    #endregion
}
