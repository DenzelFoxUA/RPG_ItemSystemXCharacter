

namespace InventoryCoreLib
{
    public interface IAmmo : IOwnedEquipable
    {
        public uint CurrentStackCount { get; }
    }
}
