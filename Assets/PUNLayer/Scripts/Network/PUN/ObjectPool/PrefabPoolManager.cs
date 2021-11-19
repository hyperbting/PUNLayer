using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPoolManager : SingletonMonoBehaviour<PrefabPoolManager>, IPunPrefabPool
{
    public List<GameObject> registeredPrefab;
    #region mono
    private void Start()
    {
        foreach (var go in registeredPrefab)
            CreatePrefabPool(go);

        // replace Photon One with myself
        //PhotonNetwork.PrefabPool = this;
    }
    #endregion

    /// <summary>Contains a GameObject per prefabId, to speed up instantiation.</summary>
    readonly Dictionary<string, PrefabPool> ResourceCache = new Dictionary<string, PrefabPool>();

    public GameObject Instantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
    {
        PrefabPool res = null;
        if (!this.ResourceCache.TryGetValue(prefabGO.name, out res))
        {
            Debug.LogError($"ObjectPool failed to load \" {prefabGO.name} \".");

            // Construct One ObjectPool
            res = CreatePrefabPool(prefabGO);
        }

        return res.GetFromPool(position, rotation);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        PrefabPool res = null;
        if (!this.ResourceCache.TryGetValue(prefabId, out res))
        {
            Debug.LogError("ObjectPool failed to load \"" + prefabId + "\".");

            // Construct One ObjectPool
            var prefabGO = Resources.Load<GameObject>(prefabId);
            if (prefabGO == null)
            {
                Debug.LogError("DefaultPool failed to load \"" + prefabId + "\". Make sure it's in a \"Resources\" folder.");
                return null;
            }

            registeredPrefab.Add(prefabGO);
            res = CreatePrefabPool(prefabGO);
        }

        return res.GetFromPool(position, rotation);
    }

    public void Destroy(GameObject gameObject)
    {
        //get parent PrefabPool
        gameObject.GetComponent<IPooledObject>()?.GetParentPool?.PutBackInPool(gameObject);
        gameObject.GetComponent<PhotonView>().ViewID = 0;
    }

    PrefabPool CreatePrefabPool(GameObject gameObject)
    {
        var go = new GameObject(gameObject.name);
        go.transform.SetParent(this.transform);
        var scr = go.AddComponent<PrefabPool>();
        scr.InitializePool(gameObject);

        this.ResourceCache.Add(gameObject.name, scr);
        return scr;
    }
}
