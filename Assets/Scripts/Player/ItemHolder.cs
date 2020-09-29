using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public string keyPrefix = "itm_";

    public GameObject PersonalItemPrefab;

    public Dictionary<string, GameObject> itemHolder = new Dictionary<string, GameObject>();

    [SerializeField]
    int itemNumber = 0;

    #region Registration
    public bool RegisterItem(string name, GameObject go)
    {
        if (itemHolder.ContainsKey(name))
            return false;

        itemHolder[name] = go;

        // now register with PlayerTransmissionToken

        return true;
    }

    public bool UnregisterItem(string name, out GameObject go)
    {
        return itemHolder.TryGetValue(name, out go);
    }
    #endregion

    public void CreateLocalItemBase()
    {
        //CreatePersonalItem
        var go = Instantiate(PersonalItemPrefab, transform.position, Quaternion.identity, transform);
        var scr = go.GetComponent<PersonalItem>();
        scr.Setup(itemNumber.ToString());
        itemHolder.Add(itemNumber.ToString(), go);

        itemNumber++;
    }

    public List<KeyValuePair<string,object>> BuildSerlizableData()
    {
        var res = new List<KeyValuePair<string, object>>();
        // Build from itemHolder to PlayerProperties

        foreach (var kvp in itemHolder)
        {
            var kes = kvp.Value.GetComponent<ISerializeData>();
            if (kes == null)
                continue;

            foreach (var dat in kes.BuildSyncData(keyPrefix + kvp.Key + "_"))
            {
                res.Add(dat);
            }
        }
        return res;
    }
}
