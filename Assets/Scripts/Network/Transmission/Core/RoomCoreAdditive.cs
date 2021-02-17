using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCoreAdditive: MonoBehaviour, ICoreAdditive
{
    [SerializeField] GameObject refObj;

    [Header("Debug")]
    [SerializeField] OwnershipSubAdditive osa;

    ITransmissionBase parent;

    public SyncTokenType AdditiveType { get { return SyncTokenType.General; } }

    public void Init(InstantiationData data, bool isMine)
    {
        this.parent = GetComponent<ITransmissionBase>();

        if (!osa)
            osa = GetComponent<OwnershipSubAdditive>();

        osa.Init(data);

        string objUUID = null;
        if (data.TryGetValue("objectuuid", out object objuuid))
        {
            objUUID = (string)objuuid;
        }

        //
        if (data.TryGetValue("localobject", out object val))
        {
            string objName = (string)val;

            var obj = ObjectManager.Instance.BuildObject(objName, objUUID);
            if (obj == null)
            {
                Debug.LogWarning("[Init] Fail to BuildObject");
                return;
            }

            refObj = obj as GameObject;

            // link between Token and TokenUser
            var tu = refObj.GetComponent<ISyncHandlerUser>();
            tu?.Init(data, false, parent);

            Debug.LogWarning("[Init] IOwnershipInteractable");
            var oi = refObj.GetComponent<IOwnershipInteractable>();
            oi.TargetObject = gameObject as object;

            Debug.LogWarning("[Init] SerializableHelper");
            parent.Register(tu.SerializableReadWrite);
            var sh = GetComponent<SerializableHelper>();
            sh.enabled = true;

        }
    }
}
