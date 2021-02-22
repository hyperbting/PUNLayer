using UnityEngine;

public class PlayerCoreAdditive : MonoBehaviour, ICoreAdditive
{
    [SerializeField] TransmissionBase parent;
    public SyncTokenType AdditiveType { get { return SyncTokenType.Player; } }

    public void Init(InstantiationData data, bool isMine)
    {
        Debug.LogWarning($"[PlayerCoreAdditive] Init isMine?{isMine}");
        this.parent = GetComponent<ITransmissionBase>() as TransmissionBase;

        if (isMine)
        {
            InitLocal();
        }
        else
            InitRemote(data);
    }

    void InitLocal()
    {
        gameObject.name = "MyPlayerToken";

        this.parent.RefObject = PlayerMaker.Instance.GetMine();
        // this is local one, do not have to give InstantiationData
    }

    void InitRemote(InstantiationData data)
    {
        Debug.LogWarning($"[PlayerCoreAdditive] InitRemote");

        gameObject.name = "RemotePlayerToken";

        //Based on objName/ objUUID we received...
        if (data.TryGetValue(InstantiationData.InstantiationKey.objectuuid, out object objuuid) &&
            data.TryGetValue(InstantiationData.InstantiationKey.objectname, out object objname))
        {
            // remote one, Create based on ObjectName
            var go = ObjectManager.Instance.BuildObject((string)objname, (string)objuuid) as GameObject;
            this.parent.RefObject = go;

            go.GetComponent<Player>().Init(data, false, parent);
        }
    }
}