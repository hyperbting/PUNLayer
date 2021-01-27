// Player
using System;

public interface ISyncHandlerUser
{
    // TokenUser must supply data about how/ what to create in remote
    InstantiationData SupplyInstantiationData { get; }

    // TokenUser MUST support how/ what to sync
    SerializableReadWrite[] SerializableReadWrite { get; }

    //local: SetupBy ObjectMaker 
    //remote: SetupBy ITransmissionBase
    void Init(InstantiationData data, bool isMine);

    // For Local, TokenUser MUST know WHERE to get TokenHandler
    void SetupTokenHandler();
}

// Player get one when created, use this to sync when IsHost
public interface ITokenHandler
{
    #region Checker
    bool HavingToken();
    #endregion

    #region
    object GetGameObject();
    #endregion

    #region Init
    void Setup(ITokenProvider itp, ISyncHandlerUser handlerUser);

    Action<InstantiationData> OnJoinedOnlineRoomEventBeforeTokenCreation { get; set; }
    //Action<ITransmissionBase> OnJoinedOnlineRoomEventAfterTokenCreation { get; set; }
    #endregion

    #region Usage
    bool PushStateInto(string key, object data);

    object CreateInRoomObject();
    bool DestroyTargetObject();

    void RequestOwnership();
    void ReleaseOwnership();
    #endregion
}

// provide ITokenHandler to ISyncHandlerUser
public interface ITokenHandlerProvider
{
    #region giving Network Ability
    ITokenHandler RequestTokenHandlerAttachment(SyncTokenType tokenType, object refScript);
    object RequestTokenHandler(SyncTokenType tokenType, object refObj);
    #endregion
}

// provide ITransmissionBase/TransmissionToken to ITokenHandler
public interface ITokenProvider
{
    #region Used by ITokenHandler
    object RequestSyncToken(InstantiationData datatoSend);
    //object RequestManualSyncToken(InstantiationData datatoSen);
    #endregion
}

// ITokenHandler Request one of this From ITokenProvider When OnJoinedRoom
public interface ITransmissionBase
{
    ISerializableHelper SeriHelper { get; }
    ISerializableHelper StatHelper { get; }

    void Register(params SerializableReadWrite[] srws);
    void Unregister(params SerializableReadWrite[] srws);
}