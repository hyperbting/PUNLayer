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
                new SerializableReadWrite("Murmer2", ReadMur2, WriteMur2),
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


    public void UpdateMurmur2()
    {
        murmur2[Random.Range(0, murmur2.Count)] = Quaternion.Euler(Random.Range(1,29), Random.Range(30,45), Random.Range(45,60));
    }

    public List<Quaternion> murmur2;
    void WriteMur2(object mm)
    {
        murmur2 = new List<Quaternion>((Quaternion[])mm);
    }

    object ReadMur2()
    {
        return murmur2.ToArray();
    }
}
