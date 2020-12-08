using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPoolManager : MonoBehaviour, IPunPrefabPool
{
    public List<GameObject> registeredPrefab;
    #region mono
    private void Start()
    {
        foreach (var go in registeredPrefab)
            CreatePrefabPool(go);

        // replace Photon One with myself
        PhotonNetwork.PrefabPool = this;
    }
    #endregion

    /// <summary>Contains a GameObject per prefabId, to speed up instantiation.</summary>
    public readonly Dictionary<string, PrefabPool> ResourceCache = new Dictionary<string, PrefabPool>();

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        PrefabPool res = null;
        if (!this.ResourceCache.TryGetValue(prefabId, out res))
        {
            Debug.LogError("ObjectPool failed to load \"" + prefabId + "\".");

            // Construct One ObjectPool

            this.ResourceCache.Add(prefabId, res);
        }

        return res.GetFromPool(position, rotation);
    }

    public void Destroy(GameObject gameObject)
    {
        //get parent PrefabPool
        gameObject.GetComponent<IPooledObject>()?.GetParentPool?.PutBackInPool(gameObject);
        gameObject.GetComponent<PhotonView>().ViewID = 0;


        ////fetch the script that implement IPooledObject
        //var scrName = gameObject.GetComponent<IPooledObject>().ToString();

        //if (!this.ResourceCache.TryGetValue(scrName, out PrefabPool res))
        //{
        //    if (res == null)
        //    {
        //        Debug.LogError("ObjectPool failed to load \"" + scrName + "\".");
        //        return;
        //    }

        //    res.PutBackInPool(gameObject);
        //}
    }

    void CreatePrefabPool(GameObject gameObject)
    {
        var ipo = gameObject.GetComponent<IPooledObject>();

        var go = new GameObject(gameObject.name);
        go.transform.SetParent(this.transform);
        var scr = go.AddComponent<PrefabPool>();
        scr.InitializePool(gameObject);

        this.ResourceCache.Add(gameObject.name, scr);
    }
}
