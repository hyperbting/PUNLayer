public interface ISyncTokenUser
{
    void RegisterWithTransmissionToken(ITransmissionBase pt);
}

public interface ITokenProvider
{
    object RequestTokenHandler(SyncTokenType tokenType, object refObj);
    object RequestSyncToken(InstantiationData datatoSend, object refObj);
}

public interface ITransmissionBase
{
    ISerializableHelper SeriHelper { get; }
    ISerializableHelper StatHelper { get; }
}