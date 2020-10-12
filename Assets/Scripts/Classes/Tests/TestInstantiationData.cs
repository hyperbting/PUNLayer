using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestInstantiationData
    {
        [Test]
        public void TestInstantiationDataSerialzation()
        {
            var data = new InstantiationData(
                new object[2] {
                    SyncTokenType.Player,
                    new List<string> {
                        "syncPos","True","syncRot","False"
                    }
                });

            Debug.Log($"data:{data.ToString()} {data.keyValuePairs.Count} {data.ToData().Length}");

            var ty = (SyncTokenType)data.ToData()[0];
            Debug.Log($"data SyncTokenType:{ty}");
            var li = (List<string>)data.ToData()[1];
            Debug.Log($"data List<string>:{li}");
            foreach( var ll in li)
                Debug.Log($"<string>:{ll}");

            var data2 = new InstantiationData(data.ToData());
            Debug.Log($"data2 Objectify:{data2.ToString()} {data2.keyValuePairs.Count} {data2.ToData().Length}");

            var data3 = InstantiationData.Build(data.ToString());
            data3.Add("k3", "v3");
            Debug.Log($"data3 Stringify:{data3.ToString()} {data3.keyValuePairs.Count} {data3.ToData().Length}");

            var data4 = InstantiationData.Build(SyncTokenType.Player);
            data4.Add("k1","v1");
            data4.Add("k2", "v2");
            data4.Add("k3", "v3");
            data4.Add("k4", "v4");
            Debug.Log($"data4 Stringify:{data4.ToString()} {data4.keyValuePairs.Count} {data4.ToData().Length}");
        }
    }
}
