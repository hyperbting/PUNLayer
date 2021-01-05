using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistExistenceAdditive : MonoBehaviourPunCallbacks
{
    ITransmissionBase parent;
    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        parent = itb;

        this.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!counting)
            return;
        endCount -= Time.deltaTime;

        if (endCount < 0)
            SelfDestroy();
    }

    public bool IsSignatureMatch(object[] instantiationData)
    {
        return false;
    }

    #region Countdown Destroy
    bool counting = false;
    [SerializeField] float endCount;
    public void StartCountdown(float ttl = 60)
    {
        if (counting)
            return;

        endCount = ttl;
        counting = true;
    }

    public void CancelCountdown()
    {
        counting = false;
    }

    public void SelfDestroy()
    {
    }
    #endregion
}
