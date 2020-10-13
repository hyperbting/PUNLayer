public interface ISyncTokenUser
{
    void RegisterWithTransmissionToken(ITransmissionBase pt);
}

public interface ITransmissionBase
{
    ISerializableHelper SeriHelper { get; }
    ISerializableHelper StatHelper { get; }
}