using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransformSubAdditive : MonoBehaviourPunCallbacks//, ITokenAdditive
{
    public Transform RefTransform;

    [Header("Appear When Sync with SyncWithPUNTranform")]
    [SerializeField] PhotonTransformView photonTV;

    [SerializeField] ITransmissionBase parent;

    void Awake()
    {
        if (photonTV != null)
            return;

        photonTV = gameObject.AddComponent<PhotonTransformView>();
        photonView.ObservedComponents.Add(photonTV);
    }

    private void Update()
    {
        if (RefTransform == null)
            return;

        // let this token follow RefTransform
        if (photonView.IsMine)
        {
            transform.position = RefTransform.position;
            transform.rotation = RefTransform.rotation;
        }
        else
        {
            RefTransform.position = transform.position;
            RefTransform.rotation = transform.rotation;
        }
    }

    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        parent = itb;
    }
}
