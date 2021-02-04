using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCoreAdditive: MonoBehaviour, ICoreAdditive
{
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

        if (isMine)
        {
            gameObject.tag = "RoomObject";

            //Load Prefab with InstantiationData data
            parent.RefObject = Load(data);
        }
    }

    GameObject Load(InstantiationData data)
    {
        //if (data.ContainsKey("RenameGO"))
        //    gameObject.name = data["RenameGO"] + $"<{photonView.ViewID}>";

        return null;
    }
}
