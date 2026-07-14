using PROJ.Communication.Results;

namespace PROJ.Tools;

public interface IPickupable
{
    bool Pickupable { get; }
    public PickUpResult? PickUp(Player player);
}