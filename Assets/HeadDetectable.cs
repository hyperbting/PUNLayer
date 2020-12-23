using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HeadDetectable : MonoBehaviour, IHeadDetectable
{

    public Action<bool, Vector3> OnRelativePositionChanged {
        get;
        set;
    }

    [SerializeField] Guid myGUID = Guid.NewGuid();
    public Guid GetUUID
    {
        get
        {
            return myGUID;
        }
    }

    [SerializeField] ControllerType conType;
    public ControllerType ConType
    {
        get
        {
            return conType;
        }
    }

    [Header("Debug Purpose")]
    [SerializeField] bool inRange;
    [SerializeField] Vector3 deltaPosition;

    [SerializeField] HeadDetection hd;
    public void Assign(HeadDetection hd)
    {
        this.hd = hd;
    }

    public void Clean()
    {
        this.hd = null;
    }

    public bool InsideHeadDetection(out Vector3 posDelta)
    {
        posDelta = Vector3.zero;

        if (hd == null)
            return false;

        var delta = hd.TryGetPositionDelta(this);
        if (delta == null)
        {
            return false;
        }

        posDelta = (Vector3)delta;
        return true;
    }

    void ChangeRelativePosition(bool enabled, Vector3 deltaPos)
    {
        Debug.Log($"{ConType} {enabled} {deltaPos}");
    }

    void UpdateDetection()
    {
        if (InsideHeadDetection(out Vector3 posDelta))
        {
            if (!inRange || Vector3.Distance(deltaPosition, posDelta)>0.1)
                OnRelativePositionChanged?.Invoke(true, posDelta);

            inRange = true;
            deltaPosition = posDelta;
        }
        else
        {
            if (inRange)
            {
                OnRelativePositionChanged?.Invoke(false, Vector3.zero);

                inRange = false;
                deltaPosition = posDelta;
            }
        }
    }

    #region Mono
    private void OnEnable()
    {
        OnRelativePositionChanged += ChangeRelativePosition;
    }

    private void OnDisable()
    {
        OnRelativePositionChanged -= ChangeRelativePosition;
    }

    private void Update()
    {
        UpdateDetection();
    }
    #endregion
}

public enum ControllerType
{
    None,
    Left,
    Right
}