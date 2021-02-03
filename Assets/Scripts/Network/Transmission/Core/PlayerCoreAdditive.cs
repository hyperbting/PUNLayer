using UnityEngine;

public class PlayerCoreAdditive : MonoBehaviour, ICoreAdditive
{
    [SerializeField] GameObject refPlayer;

    public SyncTokenType AdditiveType { get { return SyncTokenType.Player; } }

    public ISyncHandlerUser Init(InstantiationData data, bool isMine)
    {
        Debug.LogWarning($"[PlayerCoreAdditive] Init isMine?{isMine}");

        if (isMine)
        {
            InitLocal();
        }
        else
            InitRemote(data);

        return refPlayer.GetComponent<ISyncHandlerUser>();
    }

    void InitLocal()
    {
        gameObject.name = "MyPlayerToken";

        refPlayer = PlayerManager.Instance.GetHostPlayer();
        // this is local one, do not have to give InstantiationData
    }

    void InitRemote(InstantiationData data)
    {
        gameObject.name = "RemotePlayerToken";

        //Based on objName/ objUUID we received...
        if (data.TryGetValue(InstantiationData.InstantiationKey.objectuuid, out object objuuid) &&
            data.TryGetValue(InstantiationData.InstantiationKey.objectname, out object objname))
        {
            if (refPlayer != null)
            {
                Debug.LogWarning($"[PlayerCoreAdditive] RefPlayer assigned");
                return;
            }

            // remote one, Create based on ObjectName
            refPlayer = ObjectManager.Instance.BuildObject((string)objname, (string)objuuid) as GameObject;

            // remote one, pass InstantiationData on
            var istu = refPlayer.GetComponent<ISyncHandlerUser>();
            istu.Init(data, false);

            //var ish = GetComponent<ITransmissionBase>();
            //ish.Register(istu.SerializableReadWrite);

            var pa = refPlayer.GetComponent<PersistenceHelper>();
            if (pa != null)
            {
                Debug.LogWarning("[PlayerCoreAdditive] Link Token");
                pa.tBase = GetComponent<TransmissionBase>();
                pa.shouldCountdown = true;
            }
        }
    }
}