// Player
public interface ISyncHandlerUser
{
    void RegisterSyncProcess();
}

// Player get one when created, use this to sync when IsHost
public interface ITokenHandler
{
    #region Checker
    bool HavingToken();
    #endregion

    #region Init
    void Setup(ITokenProvider itp, SyncTokenType tType, object refObj);
    void Register(SyncTokenType tType, params SerializableReadWrite[] srws);
    void Unregister(SyncTokenType tType, params SerializableReadWrite[] srws);
    #endregion

    #region Usage
    bool PushStateInto(string key, object data);
    #endregion
}

// provide ITokenHandler to ISyncHandlerUser
public interface ITokenProvider
{
    object RequestTokenHandler(SyncTokenType tokenType, object refObj);
    object RequestSyncToken(InstantiationData datatoSend, object refObj);
}

// ITokenHandler Request one of this From ITokenProvider When OnJoinedRoom
public interface ITransmissionBase
{
    ISerializableHelper SeriHelper { get; }
    ISerializableHelper StatHelper { get; }
}