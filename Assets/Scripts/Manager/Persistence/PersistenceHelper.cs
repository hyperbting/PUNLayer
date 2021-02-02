using System;
using UnityEngine;

public class PersistenceHelper : MonoBehaviour
{
    public bool shouldCountdown = false;
    public TransmissionBase tBase;
    [SerializeField] string tokenID;

    Action OnDestroyEvent;

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
        if (tBase != null)
            Destroy(tBase.gameObject);
    }

    [SerializeField] float lifetime = 30;
    public void Init(float delayDestroy = 30)
    {
        lifetime = delayDestroy;
    }

    public void Init(string uuid, Action onDestroyAct, float delayDestroy = 30)
    {
        Init(delayDestroy);

        tokenID = uuid;
        OnDestroyEvent += onDestroyAct;
    }

    private void FixedUpdate()
    {
        if (!shouldCountdown || tBase != null)
            return;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        lifetime -= Time.fixedDeltaTime;
    }
}
