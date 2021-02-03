using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] bool doUpdateMurmurAsOwner;
    [SerializeField] bool doUpdateMurmur2AsOwner;
    private void Start()
    {
        InvokeRepeating("UpdateMurmur", 3, 3);
        //InvokeRepeating("UpdateMurmur2", 2, 1);
    }

    public SerializableReadWrite[] SerializableReadWrite
    {
        get
        {
            //Debug.LogWarning($"PlayerAbility SerializableReadWrite 2");
            return new SerializableReadWrite[] {
                //new SerializableReadWrite("UnitName", GetUnitName, SetUnitName),
                new SerializableReadWrite("Murmer", ReadMur, WriteMur),
                //new SerializableReadWrite("Murmer2", PrepareReadMur2(), PrepareWriteMur2()),
            };
        }
    }

    #region SerializableReadWrite:Murmer 
    public void UpdateMurmur()
    {
        if (!doUpdateMurmurAsOwner)
            return;

        murmur[Random.Range(0, murmur.Count)] = Random.Range(-1f, 1f);
    }

    public List<float> murmur;
    void WriteMur(object mm)
    {
        murmur = new List<float>((float[])mm);
    }

    object ReadMur()
    {
        return murmur.ToArray();
    }
    #endregion

    public void UpdateMurmur2()
    {
        if (!doUpdateMurmur2AsOwner)
            return;

        //murmur2[Random.Range(0, murmur2.Count)] = Quaternion.Euler(Random.Range(1, 29), Random.Range(30, 45), Random.Range(45, 60));
    }

    #region SerializableReadWrite:Murmer2 
    public List<Quaternion> murmur2;
    //System.Action<object>[] PrepareWriteMur2()
    //{
    //    //Debug.LogWarning($"PrepareWriteMur2 {murmur2.Count}");
    //    var result = new System.Action<object>[murmur2.Count];
    //    for (int i = 0; i < result.Length; i++)
    //    {
    //        result[i] = (obj) => {
    //            murmur2[i] = (Quaternion)obj;
    //        }; 
    //    }

    //    return result;
    //}

    //System.Func<object>[] PrepareReadMur2()
    //{
    //    //Debug.LogWarning($"PrepareReadMur2 {murmur2.Count}");
    //    var result = new System.Func<object>[murmur2.Count];
    //    for (int i = 0; i < result.Length; i++)
    //    {
    //        result[i] = () => {
    //            Debug.LogWarning($"PrepareReadMur2 {murmur2.Count}");
    //            if(i < murmur2.Count )
    //                return murmur2[i];

    //            return null;
    //        };
    //    }

    //    return result;
    //}
    #endregion
}
