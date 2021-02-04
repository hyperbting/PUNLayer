using UnityEngine;

public interface IPlayerMaker
{
    GameObject GetHostPlayer();

    GameObject InstantiatePlayerObject();
    GameObject InstantiateRemotePlayerObject(string uuid, Transform parent);

    //void SyncPersonalItems();
}
