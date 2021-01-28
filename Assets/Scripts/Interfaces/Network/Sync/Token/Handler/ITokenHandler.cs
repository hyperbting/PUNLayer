using System;

// Player get one when created, use this to sync when IsHost
public interface ITokenHandler
{
    #region Checker
    bool HavingToken();
    #endregion

    #region
    object GetGameObject();
    #endregion

    #region Init
    void Setup(ITokenProvider itp, ISyncHandlerUser handlerUser);

    Action<InstantiationData> OnJoinedOnlineRoomEventBeforeTokenCreation { get; set; }
    //Action<ITransmissionBase> OnJoinedOnlineRoomEventAfterTokenCreation { get; set; }
    #endregion

    #region Usage
    bool PushStateInto(string key, object data);

    object CreateInRoomObject();
    bool DestroyTargetObject();

    void RequestOwnership();
    void ReleaseOwnership();
    #endregion
}