using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RoomCoreAdditive : MonoBehaviourPunCallbacks//, ITokenAdditive
{
    public GameObject refObject;

    ITransmissionBase parent;

    public void Init(ITransmissionBase itb, InstantiationData data)
    {
        parent = itb;

        gameObject.tag = "RoomObject";

        //Load Prefab with InstantiationData data
        refObject = Load(data);

        if (photonView.IsMine)
        {
        }
        else
        {
            //// based on InstantiationData load proper prefab
            //SetupSync(data);

            //var istu = RefPlayer.GetComponent<ISyncTokenUser>();
            //istu.RegisterWithTransmissionToken(itb);
        }
    }

    GameObject Load(InstantiationData data)
    {
        if (data.ContainsKey("RenameGO"))
            gameObject.name = data["RenameGO"] + $"<{photonView.ViewID}>";

        return null;
    }
}
