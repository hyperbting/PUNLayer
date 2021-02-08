using System.Threading.Tasks;

public interface IOwnershipInteractable
{
    bool IsMine();

    Task<bool> RequestOwnership(int acterNumber);
    void ReleaseOwnership();
}
