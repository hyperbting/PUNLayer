using UnityEngine;

public interface IPlayerMaker
{
    GameObject GetHostPlayer();

    GameObject InstantiatePlayerObject();
    GameObject InstantiateRemotePlayerObject(string uuid);

    void SyncPersonalItems();
}
