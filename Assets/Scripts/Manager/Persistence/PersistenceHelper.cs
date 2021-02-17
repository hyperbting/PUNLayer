using System;
using UnityEngine;

public class PersistenceHelper : MonoBehaviour
{
    [SerializeField] bool shouldCountdown = false;
    [SerializeField] string tokenID;

    Action OnDestroyEvent;

    [SerializeField] TransmissionBase refToken;
    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
        if (refToken)
            Destroy(refToken.gameObject);
    }

    [SerializeField] float lifetime = 30;
    public void Init(string uuid, Action onDestroyAct=null, TransmissionBase refToken=null)
    {
        Debug.Log($"PersistenceHelper Init");
        tokenID = uuid;

        if (onDestroyAct != null)
            OnDestroyEvent += onDestroyAct;

        if (refToken)
            this.refToken = refToken;
    }

    public void Setup(float delayDestroy = 30)
    {
        lifetime = delayDestroy;
        if (delayDestroy >= 0)
        {
            this.shouldCountdown = true;
            this.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (!shouldCountdown || refToken)
            return;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        lifetime -= Time.fixedDeltaTime;
    }
}
