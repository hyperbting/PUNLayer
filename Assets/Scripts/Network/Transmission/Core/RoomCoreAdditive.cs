using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCoreAdditive: MonoBehaviour, ICoreAdditive
{
    public GameObject refObject;

    public SyncTokenType AdditiveType { get { return SyncTokenType.General; } }

    public void Init(InstantiationData data, bool isMine)
    {
        if (isMine)
        {
            gameObject.tag = "RoomObject";

            //Load Prefab with InstantiationData data
            refObject = Load(data);
        }
    }

    GameObject Load(InstantiationData data)
    {
        //if (data.ContainsKey("RenameGO"))
        //    gameObject.name = data["RenameGO"] + $"<{photonView.ViewID}>";

        return null;
    }
}
