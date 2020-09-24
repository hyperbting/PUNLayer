using UnityEngine;

public interface IPlayerMaker
{
    GameObject InstantiatePlayerObject();
    GameObject InstantiateRemotePlayerObject(string uuid);

    void SyncPersonalItems();
}
