using System.Threading.Tasks;

public interface IOwnershipInteractable
{
    Task<bool> RequestOwnership(int acterNumber);
    Task<bool> ReleaseOwnership();
}
