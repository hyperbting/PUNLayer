using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HeadDetection : MonoBehaviour
{
    Dictionary<System.Guid, (GameObject,Vector3)> detectedIHeadDetectable = new Dictionary<System.Guid, (GameObject, Vector3)>();

    private void OnTriggerEnter(Collider other)
    {
        var itf = other.gameObject.GetComponent<IHeadDetectable>();
        if (itf == null)
            return;

        itf.Assign(this);
        detectedIHeadDetectable[itf.GetUUID] = (other.gameObject, Vector3.zero);
    }

    private void OnTriggerExit(Collider other)
    {
        var itf = other.gameObject.GetComponent<IHeadDetectable>();
        if (itf == null)
            return;

        UnRegister(itf);
    }

    public void UnRegister(IHeadDetectable itf)
    {
        itf.Clean();
        detectedIHeadDetectable.Remove(itf.GetUUID);
    }

    private void FixedUpdate()
    {
        var keyList = new List<System.Guid>(detectedIHeadDetectable.Keys);
        foreach (var key in keyList)
        {
            var val = detectedIHeadDetectable[key];
            var deltaPos = val.Item1.transform.position - transform.position;
            detectedIHeadDetectable[key] = (val.Item1, deltaPos);
        }
    }

    public Vector3? TryGetPositionDelta(IHeadDetectable ihd)
    {
        if (detectedIHeadDetectable.TryGetValue(ihd.GetUUID, out (GameObject, Vector3) val))
        {
            return val.Item2;
        }

        return null;
    }
}

public interface IHeadDetectable
{
    System.Guid GetUUID { get; }
    ControllerType ConType { get; }

    System.Action<bool, Vector3> OnRelativePositionChanged { get; set; }

    void Assign(HeadDetection hd);
    void Clean();
}

