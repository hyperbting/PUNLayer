// Player
using System;

public interface ISyncHandlerUser
{
    void SetupSync(ITransmissionBase itb, InstantiationData data);
}

// Player get one when created, use this to sync when IsHost
public interface ITokenHandler
{
    #region Checker
    bool HavingToken();
    #endregion

    #region Init
    void Setup(ITokenProvider itp, SyncTokenType tType, object refObj);
    void Register(params SerializableReadWrite[] srws);
    void Unregister(params SerializableReadWrite[] srws);

    Action<InstantiationData> OnJoinedOnlineRoomEventBeforeTokenCreation { get; set; }
    //Action<ITransmissionBase> OnJoinedOnlineRoomEventAfterTokenCreation { get; set; }
    #endregion

    #region Usage
    bool PushStateInto(string key, object data);

    void CreateInRoomObject();
    void RequestOwnership();
    void ReleaseOwnership();
    #endregion
}

// provide ITokenHandler to ISyncHandlerUser
public interface ITokenProvider
{
    object RequestTokenHandler(SyncTokenType tokenType, object refObj);
    object RequestSyncToken(InstantiationData datatoSend, object refObj);
    object RequestManualSyncToken(InstantiationData datatoSen);
}

// ITokenHandler Request one of this From ITokenProvider When OnJoinedRoom
public interface ITransmissionBase
{
    ISerializableHelper SeriHelper { get; }
    ISerializableHelper StatHelper { get; }

    void Register(params SerializableReadWrite[] srws);
    void Unregister(params SerializableReadWrite[] srws);
}

//public interface ITokenAdditive
//{
//    void Init(ITransmissionBase itb, InstantiationData data);
//}