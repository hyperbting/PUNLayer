using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCoreAdditive: MonoBehaviour, ICoreAdditive
{
    [Header("Debug")]
    [SerializeField] OwnershipSubAdditive osa;

    TransmissionBase parent;

    public SyncTokenType AdditiveType { get { return SyncTokenType.General; } }

    public void Init(InstantiationData data, bool isMine)
    {
        Debug.LogWarning("[RoomCoreAdditive] Init");
        this.parent = GetComponent<TransmissionBase>();

        if (!osa)
            osa = GetComponent<OwnershipSubAdditive>();

        osa.Init(data);

        //
        if (data.TryGetValue(InstantiationData.InstantiationKey.objectuuid, out object objUUID) &&
            data.TryGetValue(InstantiationData.InstantiationKey.objectname, out object objName))
        {
            var obj = ObjectSupplyManager.Instance.BuildObject((string)objName, (string)objUUID);
            if (obj == null)
            {
                Debug.LogError("[Init] Fail to BuildObject");
                return;
            }

            var go = obj as GameObject;
            parent.RefObject = go;
            go.transform.SetParent(ServiceManager.Instance.networkSystem.RoomObjectParent);

            // link between Token and TokenUser
            var tu = go.GetComponent<ISyncHandlerUser>();
            tu?.Init(data, false, parent);

            Debug.LogWarning("[Init] IOwnershipInteractable");
            var oi = go.GetComponent<IOwnershipInteractable>();
            oi.TargetObject = gameObject as object;

            Debug.LogWarning("[Init] SerializableHelper");
            parent.Register(tu.SerializableReadWrite);
            var sh = GetComponent<SerializableHelper>();
            sh.enabled = true;

        }
    }
}
