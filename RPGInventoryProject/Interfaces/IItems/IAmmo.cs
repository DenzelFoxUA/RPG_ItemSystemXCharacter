

namespace InventoryCoreLib
{
    public interface IAmmo : IOwnedEquipable, IStackable
    {
        public string GetAmmoType();
    }
}
