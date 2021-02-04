public interface ISyncHandlerUser
{
    // TokenUser must supply data about how/ what to create in remote
    InstantiationData SupplyInstantiationData { get; }

    // TokenUser MUST support how/ what to sync
    SerializableReadWrite[] SerializableReadWrite { get; }

    //local: SetupBy ObjectMaker, setup TokenHalnder
    //remote: SetupBy ITransmissionBase
    void Init(InstantiationData data, bool isMine, ITransmissionBase itb);

    object GameObject();
}