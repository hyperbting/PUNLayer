using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
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
        ObjectManager.Instance.RegisterBuilder(BuildLocalPlayerObject);

        if(createPlayerOnStart)
            InstantiatePlayerObject();
    }

    private void OnDestroy()
    {
        ObjectManager.Instance.UnregisterBuilder(BuildLocalPlayerObject);
    }

    public GameObject GetHostPlayer()
    {
        return hostPlayer.gameObject;
    }

    public GameObject InstantiatePlayerObject()
    {
        if (hostPlayer != null)
        { 
            Debug.LogWarning($"Player Exist!");
            return GetHostPlayer();
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
            insData["timestamp"] = Time.time;
            insData[InstantiationData.InstantiationKey.objectname.ToString()] = playerCorePref.name;
            insData[InstantiationData.InstantiationKey.objectuuid.ToString()] = randomID;
            insData["ablePlayerEcho"] = true;

            playerScript.Init(insData, true);
            playerScript.SetupTokenHandler();
        }
        Debug.LogWarning($"InstantiatePlayerObject HostPlayer Created");
        return go;
    }

    Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
    public GameObject BuildLocalPlayerObject(string objName, string UUID = null)
    {
        Debug.LogWarning($"BuildLocalPlayerObject Start");

        GameObject go = null;
        PersistenceHelper ph = null;
        switch (objName)
        {
            case "Player":
                ////LookUp before Create
                if (UUID != null && dic.TryGetValue(UUID, out go))
                {
                    ph = go.GetComponent<PersistenceHelper>();
                    ph.Init();
                    return go;
                }

                go = Instantiate(playerCorePref, transform);
                dic[UUID] = go;

                ph = go.GetComponent<PersistenceHelper>();
                ph.Init(UUID, () => {
                    dic.Remove(UUID);
                    Debug.Log($"Remove {UUID} from Dic");
                });

                break;
        }

        Debug.LogWarning($"BuildLocalPlayerObject RemotePlayer Created");
        return go;
    }

}
