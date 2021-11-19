using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] GameObject targetObject;
    public object TargetObject
    {
        get {
            return targetObject;
        }

        set {
            targetObject = value as GameObject;
        }
    }

    public bool IsMine()
    {
        if (TargetObject == null)
            return false;

        var ioi = targetObject.GetComponent<IOwnershipInteractable>();
        if (ioi == null)
            return false;

        return ioi.IsMine();
    }

    public int GetNetworkID()
    {
        if (TargetObject == null)
            return -1;

        var ioi = targetObject.GetComponent<IOwnershipInteractable>();
        if (ioi == null)
            return -1;

        return ioi.GetNetworkID();
    }

    public async Task<bool> RequestOwnership(int acterNumber)
    {
        if (!targetObject)
        {
            Debug.LogWarning("[InteractiableHelper] RequestOwnership targetObject null");
            return false;
        }

        var ioi = targetObject.GetComponent<IOwnershipInteractable>();
        if (ioi == null)
        {
            Debug.LogWarning("[InteractiableHelper] RequestOwnership IOwnershipInteractable null");
            return false;
        }

        Debug.Log($"[InteractiableHelper] RequestOwnership {targetObject.name} {acterNumber}");
        return await ioi.RequestOwnership(acterNumber);
    }

    public void ReleaseOwnership()
    {
        if (!targetObject)
        {
            Debug.LogWarning("[InteractiableHelper] ReleaseOwnership targetObject null");
            return;
        }

        var ioi = targetObject.GetComponent<IOwnershipInteractable>();
        if (ioi == null)
        {
            Debug.LogWarning("[InteractiableHelper] ReleaseOwnership IOwnershipInteractable null");
            return;
        }

        Debug.Log($"[InteractiableHelper] ReleaseOwnership {targetObject.name}");
        ioi.ReleaseOwnership();

    }

    public void TryRequestOwnership()
    {
        _ = RequestOwnership(ServiceManager.Instance.networkSystem.GetNetworkID());
    }

    public void MousePointer(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
        {
            var ioi = hit.transform.GetComponent<IOwnershipInteractable>();
            if (ioi == null)
                return;

            var go = hit.transform.gameObject;
            Debug.Log($"{go.name} Selected");
            targetObject = go;
        }

        //var targets = Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePosition), Mathf.Infinity, LayerMask.GetMask("NetworkView"));
        //foreach (var ta in targets)
        //{
        //    //Debug.Log($"{ta.transform.name}");
        //    var pv = Photon.Pun.PhotonView.Get(ta.collider);
        //    if (pv != null && Photon.Pun.UtilityScripts.PointedAtGameObjectInfo.Instance != null)
        //        Photon.Pun.UtilityScripts.PointedAtGameObjectInfo.Instance.SetFocus(pv);
        //}
    }
}
