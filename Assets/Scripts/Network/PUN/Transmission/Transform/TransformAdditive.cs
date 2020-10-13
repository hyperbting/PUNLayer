using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAdditive : MonoBehaviourPunCallbacks
{
    public Transform RefTransform;

    [Header("Appear When Sync with SyncWithPUNTranform")]
    [SerializeField] PhotonTransformView photonTV;

    [SerializeField] TransmissionBase transBase;

    void Awake()
    {
        transBase = GetComponent<TransmissionBase>();

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
}
