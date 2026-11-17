

namespace InventoryCoreLib
{
    public interface IAmmo : IOwnedEquipable
    {
        public uint CurrentStackCount { get; }
        public string GetAmmoType();
        public void UseOne();
    }
}
