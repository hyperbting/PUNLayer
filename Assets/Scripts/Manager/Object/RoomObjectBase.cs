using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomObjectBase : MonoBehaviour, ISyncHandlerUser, IOwnershipInteractable
{
    [SerializeField] GameObject refToken;

    #region IOwnershipInteractable
    public bool IsMine()
    {
        if (!refToken)
            return false;

        //Check with token Owner
        return refToken.GetComponent<IOwnershipInteractable>().IsMine();
    }

    public object TargetObject
    {
        get
        {
            return refToken;
        }
        set
        {
            refToken = value as GameObject;
        }
    }

    public async Task<bool> RequestOwnership(int acterNumber)
    {
        if (!refToken)
            return false;

        return await refToken.GetComponent<IOwnershipInteractable>().RequestOwnership(acterNumber);
    }

    public void ReleaseOwnership()
    {
        if (!refToken)
            return;

        refToken.GetComponent<IOwnershipInteractable>().ReleaseOwnership();
    }
    #endregion

    #region ISyncHandlerUser
    [SerializeField] InstantiationData instData;
    public InstantiationData SupplyInstantiationData {
        get {
            return instData;
        }
    }

    public SerializableReadWrite[] SerializableReadWrite
    {
        get
        {
            var local = new SerializableReadWrite[] {
                new SerializableReadWrite("Pos", ReadPos, WritePos),
                new SerializableReadWrite("Rot", ReadRot, WriteRot),
            };

            return local;
        }
    }

public object GameObject()
    {
        throw new System.NotImplementedException();
    }

    public void Init(InstantiationData data, bool isMine, ITransmissionBase itb)
    {
        throw new System.NotImplementedException();
    }
    #endregion

#region SerilizableReadWrite
    void WritePos(object pos)
    {
        //Debug.Log($"WritePos {pos}");
        refToken.transform.position = (Vector3)pos;
    }

    object ReadPos()
    {
        //Debug.Log($"ReadPos {refTransform.position}");
        return refToken.transform.position;
    }

    void WriteRot(object rot)
    {
        refToken.transform.rotation = (Quaternion)rot;
    }

    object ReadRot()
    {
        return refToken.transform.rotation;
    }
#endregion SerilizableReadWrite
}
