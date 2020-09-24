using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public GameObject PersonalItemPrefab;
    public Dictionary<string, GameObject> stateHolder = new Dictionary<string, GameObject>();

    [SerializeField]
    int itemNumber = 0;

    #region Registration
    public bool RegisterItem(string name, GameObject go)
    {
        if (stateHolder.ContainsKey(name))
            return false;

        stateHolder[name] = go;

        // now register with PlayerTransmissionToken

        return true;
    }

    public bool UnregisterItem(string name, out GameObject go)
    {
        return stateHolder.TryGetValue(name, out go);
    }
    #endregion

    public void CreateItemBase()
    {
        //CreatePersonalItem
        var go = Instantiate(PersonalItemPrefab, transform.position, Quaternion.identity, transform);
        var scr = go.GetComponent<PersonalItem>();
        scr.Setup(itemNumber.ToString());
        stateHolder.Add(itemNumber.ToString(), go);

        itemNumber++;
    }
}
