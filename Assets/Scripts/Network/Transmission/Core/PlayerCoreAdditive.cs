using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoreAdditive : MonoBehaviour, ICoreAdditive
{
    [SerializeField] GameObject RefPlayer;

    public SyncTokenType AdditiveType { get { return SyncTokenType.Player; } }

    public void Init(InstantiationData data, bool isMine)
    {
        Debug.LogWarning($"[PlayerCoreAdditive] Init isMine?{isMine}");

        if (isMine)
        {
            gameObject.name = "MyPlayerToken";
            RefPlayer = PlayerManager.Instance.GetHostPlayer();
            // this is local one, do not have to give InstantiationData
        }
        else
            InitRemote(data);
    }

    void InitRemote(InstantiationData data)
    {
        gameObject.name = "RemotePlayerToken";

        //Based on objName/ objUUID we received...
        if (data.TryGetValue(InstantiationData.InstantiationKey.objectuuid, out object objuuid) &&
            data.TryGetValue(InstantiationData.InstantiationKey.objectname, out object objname))
        {
            if (RefPlayer != null)
            {
                Debug.LogWarning($"[PlayerCoreAdditive] RefPlayer assigned");
                return;
            }

            // this is remote one, pass on InstantiationData
            RefPlayer = ObjectManager.Instance.BuildObject((string)objname, (string)objuuid) as GameObject;

            var istu = RefPlayer.GetComponent<ISyncHandlerUser>();
            istu.Init(data, false);

            var ish = GetComponent<ITransmissionBase>();
            ish.Register(istu.SerializableReadWrite);
            //ish.enabled = true;

            var pa = RefPlayer.GetComponent<PersistenceHelper>();
            if (pa != null)
            {
                Debug.LogWarning("[PlayerCoreAdditive] Link Token");
                pa.tBase = GetComponent<TransmissionBase>();
                pa.shouldCountdown = true;
            }
        }
    }
}