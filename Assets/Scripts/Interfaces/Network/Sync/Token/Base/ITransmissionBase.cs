// ITokenHandler Request one of this From ITokenProvider When OnJoinedRoom
public interface ITransmissionBase
{
    /// <summary>
    /// Setup By TokenHandler locally 
    /// or 
    /// ITransmissionBase remotely
    /// </summary>
    /// <param name="insData"></param>
    /// <param name="tokenUser">null when remotely; TokenHandler will always assign one</param>
    void Setup(InstantiationData insData, ISyncHandlerUser tokenUser = null);

    ISerializableHelper SeriHelper { get; }
    ISerializableHelper StatHelper { get; }

    void Register(params SerializableReadWrite[] srws);
    void Unregister(params SerializableReadWrite[] srws);
}
