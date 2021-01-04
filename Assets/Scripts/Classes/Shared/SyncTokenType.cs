public enum SyncTokenType
{
    Unknown,
    General,
    /// <summary>
    /// Owner Creation
    /// </summary>
    Player,
    /// <summary>
    /// SceneOwned, NOT destroy when controller left, countdown to destroy; 
    /// NOT NetworkInstantiate BUT RaiseEvent, MC Clean RaiseEvent when countdown Finished
    /// </summary>
    Persistence,
}
