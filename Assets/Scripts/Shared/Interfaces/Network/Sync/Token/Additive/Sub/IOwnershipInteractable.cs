using System.Threading.Tasks;

public interface IOwnershipInteractable
{
    bool IsMine();

    object TargetObject { get; set; }

    Task<bool> RequestOwnership(int acterNumber);
    void ReleaseOwnership();
}
