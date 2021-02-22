using System.Threading.Tasks;

public interface IOwnershipInteractable
{
    bool IsMine();

    int GetNetworkID();

    object TargetObject { get; set; }

    Task<bool> RequestOwnership(int acterNumber);
    void ReleaseOwnership();
}
