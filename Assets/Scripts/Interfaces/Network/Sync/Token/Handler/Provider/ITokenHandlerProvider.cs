// provide ITokenHandler to ISyncHandlerUser
public interface ITokenHandlerProvider
{
    #region giving Network Ability
    ITokenHandler RequestTokenHandlerAttachment(SyncTokenType tokenType, object refScript);

    object RequestTokenHandler(SyncTokenType tokenType, object refObj);
    #endregion
}