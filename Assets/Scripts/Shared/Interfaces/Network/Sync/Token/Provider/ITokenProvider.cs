// provide ITransmissionBase/TransmissionToken to ITokenHandler
public interface ITokenProvider
{
    #region Used by ITokenHandler
    object RequestSyncToken(InstantiationData datatoSend);

    void RevokeSyncToken(InstantiationData insData);
    void RevokeSyncToken(int networkID);
    #endregion
}
