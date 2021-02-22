// ITokenHandler Request one of this From ITokenProvider When OnJoinedRoom
public interface ITransmissionBase
{
    /// <summary>
    /// Setup By TokenHandler locally 
    /// or 
    /// ITransmissionBase remotely
    /// </summary>
    /// <param name="insData"></param>
    void Setup(InstantiationData insData);

    ISerializableHelper SeriHelper { get; }
    ISerializableHelper StatHelper { get; }

    //void Register(params SerializableReadWrite[] srws);
    //void Unregister(params SerializableReadWrite[] srws);

    object RefObject { get; set; }
}
