// provide ITransmissionBase/TransmissionToken to ITokenHandler
public interface ITokenProvider
{
    #region Used by ITokenHandler
    object RequestSyncToken(InstantiationData datatoSend);
    //object RequestManualSyncToken(InstantiationData datatoSen);
    #endregion
}
