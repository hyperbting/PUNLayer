using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestSerializable
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestSerializableSimplePasses()
        {
            var starter = new UNative()
            {
                zz = 1.1f,
                v3 = Vector3.back,
                v3s = new List<Vector3>() { Vector3.down, Vector3.up },
                q4 = Quaternion.identity,
                q4s = new List<Quaternion>() {Quaternion.identity, new Quaternion(1,2,3,4) }
            };

            var starterTxt = JsonUtility.ToJson(starter);
            Debug.Log($"StartTxt {starterTxt}");

            UNative starter2 = JsonUtility.FromJson<UNative>(starterTxt);
            var starterTxt2 = JsonUtility.ToJson(starter2);
            Debug.Log($"StartTxt2 {starterTxt2}");

            Assert.AreNotSame(starterTxt, starterTxt2, "Two Set ToJson Should Be The Same!!");
        }
    }

    [System.Serializable]
    public class UNative
    {
        public float zz;
        public Vector3 v3;
        public List<Vector3> v3s;
        public Quaternion q4;
        public List<Quaternion> q4s;
    }
}
