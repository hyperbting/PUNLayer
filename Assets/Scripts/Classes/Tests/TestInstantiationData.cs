﻿using System.Collections;
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
            var kvp = new KVP("syncPos", "True");
            Debug.Log($"kvp:{kvp.ToString()}");

            Debug.Log($"kvp2:{KVP.Build(kvp.ToString())}");

            var data = new InstantiationData(
                new object[2] {
                    SyncTokenType.Player,
                    new List<KVP> {
                        new KVP("syncPos","True"),
                        new KVP("syncRot","False")
                    }
                });

            Debug.Log($"data:{data.ToString()} {data.keyValuePairs.Count} {data.ToData().Length}");
            Debug.Log($"data Object[]:{data.ToData()}");
            foreach(var obj in data.ToData())
                Debug.Log($"data Object:{obj.ToString()}");

            var ty = (SyncTokenType)data.ToData()[0];
            Debug.Log($"data SyncTokenType:{ty}");
            var li = (List<KVP>)data.ToData()[1];
            Debug.Log($"data List<KVP>:{li}");
            foreach( var ll in li)
                Debug.Log($"<KVP>:{ll}");

            var data2 = new InstantiationData(data.ToData());
            Debug.Log($"data2 Objectify:{data2.ToString()} {data2.keyValuePairs.Count} {data2.ToData().Length}");

            var data3 = InstantiationData.Build(data.ToString());
            Debug.Log($"data3 Stringify:{data3.ToString()} {data3.keyValuePairs.Count} {data3.ToData().Length}");
        }
    }
}
