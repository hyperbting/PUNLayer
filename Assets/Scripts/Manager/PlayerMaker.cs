using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaker : SingletonMonoBehaviour<PlayerMaker>, IObjectSupplier
{
    public GameObject playerCorePref;

    [Header("Debug")]
    [SerializeField]
    Player hostPlayer;

    [SerializeField] bool createPlayerOnStart = false;

    public string randomID;

    // Start is called before the first frame update
    void Start()
    {
        randomID = Random.Range(1000, 9999).ToString();
        ObjectManager.Instance.RegisterObjectSupplier(this);

        if(createPlayerOnStart)
            InstantiateObject();
    }

    private void OnDestroy()
    {
        ObjectManager.Instance.UnregisterObjectSupplier(this);
    }

    public GameObject GetMine()
    {
        return hostPlayer?.gameObject;
    }

    public GameObject InstantiateObject()
    {
        if (hostPlayer != null)
        { 
            Debug.LogWarning($"Player Exist!");
            return GetMine();
        }

        Debug.LogWarning($"InstantiatePlayerObject Start");
        var go = Instantiate(playerCorePref, transform);

        go.name = "HostPlayer";
        var playerScript = go.GetComponent<Player>();
        if (playerScript != null)
        {
            hostPlayer = playerScript;
            playerScript.isHost = true;

            var insData = InstantiationData.Build(SyncTokenType.Player);
            insData[InstantiationData.InstantiationKey.objectname.ToString()] = playerCorePref.name;
            insData[InstantiationData.InstantiationKey.objectuuid.ToString()] = randomID;
            insData[InstantiationData.InstantiationKey.objectpersist.ToString()] = 30;
            insData["ablePlayerEcho"] = true;
            insData["timestamp"] = Time.time;

            playerScript.Init(insData, true, null);
            playerScript.creator = this;
        }
        Debug.LogWarning($"InstantiatePlayerObject HostPlayer Created");
        return go;
    }

    public void DestroyObject()
    {
        if (hostPlayer == null)
        {
            Debug.LogWarning($"HostPlayerNotExist!");
            return;
        }

        Destroy(GetMine());
        hostPlayer = null;
    }

    Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
    public object BuildLocalObject(string objName, string UUID)
    {
        Debug.LogWarning($"BuildLocalPlayerObject Start");

        GameObject go = null;
        switch (objName)
        {
            case "Player":
                ////LookUp before Create
                if (UUID != null && dic.TryGetValue(UUID, out go))
                {
                    return go;
                }

                // Create one
                go = Instantiate(playerCorePref, transform);
                dic[UUID] = go;

                var playerScript = go.GetComponent<Player>();
                playerScript.creator = this;

                break;
        }

        Debug.LogWarning($"BuildLocalPlayerObject RemotePlayer Created");
        return go;
    }

    void IObjectSupplier.DestroyLocalObject(string objName, string UUID)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveFromDict(string UUID)
    {
        dic.Remove(UUID);
        Debug.Log($"Remove {UUID} from Dic");
    }
}
