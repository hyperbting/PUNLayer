public interface ICoreAdditive
{
    SyncTokenType AdditiveType { get; }
    void Init(InstantiationData data, bool isMine);
}
