using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ExitGames.Client.Photon;
namespace Tests
{
    public class TestPunHashtableExtension
    {
        [Test]
        public void TestHashtableCleanInsert()
        {
            var ht = new ExitGames.Client.Photon.Hashtable();
            ht["Invalid"] = true;

            var kvPair = new KeyObjectPair[]
            {
                new KeyObjectPair() {k = "valid", v = true},
                new KeyObjectPair() {k = "invalid", v = false}
            };
            
            ht.CleanInsert(kvPair);
            Assert.False(ht.ContainsKey("Invalid"));
            Assert.IsNull(ht["Invalid"]);
            Assert.True((bool)ht["valid"]);
            Assert.False((bool)ht["invalid"]);
            
            Debug.Log("Test Hashtable CleanInsert Success");
        }

        [Test]
        public void TestHashtableCleanInsertWithRef()
        {
            var ht = new ExitGames.Client.Photon.Hashtable();
            ht["Invalid"] = true;

            var kvPair = new KeyObjectPair[]
            {
                new KeyObjectPair() {k = "valid", v = true},
                new KeyObjectPair() {k = "invalid", v = false}
            };

            var refht = new ExitGames.Client.Photon.Hashtable();
            refht["valid"] = true;
            refht["invalid22"] = 123;

            ht.CleanInsertDefaultNull(refht, kvPair);
            Assert.False(ht.ContainsKey("Invalid"));
            Assert.True((bool) ht["valid"]);
            Assert.False(ht.ContainsKey("invalid22"));
            Assert.IsNull(ht["invalid"]);

            Debug.Log("Test Hashtable CleanInsertDefaultNull Success");
        }
        
        [Test]
        public void TestHashtableToStringKeys()
        {
            var ht = new ExitGames.Client.Photon.Hashtable();
            var li01 = ht.StringKeys();
            Assert.NotNull(li01);
            Assert.AreEqual(0, li01.Count);

            ht["Invalid"] = true;
            ht["valid"] = true;
            ht["invalid22"] = 123;
            
            var li02 = ht.StringKeys();
            Assert.NotNull(li02);
            Assert.AreEqual(3,li02.Count);

            Debug.Log("Test Hashtable To StringKeys Success");
        }

        [Test]
        public void TestHashtableStringKeysIntersect()
        {
            var ht = new ExitGames.Client.Photon.Hashtable();
            ht["Invalid"] = true;
            ht["valid"] = true;
            
            ht["invalid22"] = 123;
            ht["invalid4522"] = 123;
            
            var dic = new Dictionary<string, object>();
            dic["Invalid"] = false;
            dic["valid"] = false;
            
            dic["invalid"] = false;
            dic["invalid22222"] = 123;

            var keli = ht.StringKeysIntersect(dic);
            
            Assert.NotNull(keli);
            Assert.AreEqual(2,keli.Count );
            foreach(var ke in keli)
                Debug.Log($"{ke}");
            Debug.Log("Test Hashtable StringKeysIntersect");
        }
        
        [Test]
        public void TestHashtableToDictionaryStringObject()
        {
            var ht = new ExitGames.Client.Photon.Hashtable();
            ht["Invalid"] = true;
            ht["valid"] = true;
            ht["invalid22"] = 123;
            ht["invalid4522"] = 123;

            var dicStrObj = ht.ToDictionaryStringObject();
            Assert.NotNull(dicStrObj);
            Assert.AreEqual(4, dicStrObj.Count);
            foreach(var ke in dicStrObj)
                Debug.Log($"{ke} ({ke.Value.GetType()}");
            
            Debug.Log("Test Hashtable ToDictionaryStringObject");
        }
    }
    
}
