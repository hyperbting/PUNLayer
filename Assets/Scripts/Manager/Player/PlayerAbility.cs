using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public SerializableReadWrite[] SerializableReadWrite
    {
        get
        {
            return new SerializableReadWrite[] {
                //new SerializableReadWrite("UnitName", GetUnitName, SetUnitName),
                new SerializableReadWrite("Murmer", ReadMur, WriteMur),
            };
        }
    }

    public void UpdateMurmur()
    {
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
}
