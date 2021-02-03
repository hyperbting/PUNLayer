public interface ICoreAdditive
{
    SyncTokenType AdditiveType { get; }
    ISyncHandlerUser Init(InstantiationData data, bool isMine);
}
