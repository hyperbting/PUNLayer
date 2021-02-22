public interface IOwnershipHandler
{
    void RequestOwnership(object targetObject);
    void ReleaseOwnership(object targetObject);
}
