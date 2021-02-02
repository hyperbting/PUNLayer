public enum SyncTokenType
{
    Unknown,
    /// <summary>
    /// Scene object, won't disappear when owner gone, disappear when LeftRoom
    /// </summary>
    General,
    /// <summary>
    /// Player Object, disappear when owner gone, disappear when LeftRoom
    /// Structure is PlayerDataCreateion below TransmissionToken
    /// </summary>
    Player,
    /// <summary>
    /// LOCAL Object, NOT destroy because of LeftRoom or ControllerLeft
    /// Structure is PlayerDataCreateion and TransmissionToken side by side.
    /// Two Parts Composition:
    /// 1. SemiPersistenceManager create/control DataCreateion
    /// 2. DataCreateion link to TransmissionToken; TransmissionToken gone when not in room.
    /// Whenever DataCreateion cannot find a TransmissionToken linked, Countdown to destroy.
    /// whenever Someone request a DataCreateion, SemiPersistenceManager find in the list or create a local DataCreateion.
    /// </summary>
    SemiPersistence,
}
