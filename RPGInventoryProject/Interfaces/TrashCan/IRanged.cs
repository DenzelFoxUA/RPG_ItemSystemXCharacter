
namespace InventoryCoreLib
{
    public interface IRanged
    {
        //public void Reload();
        //public bool CanUseAmmo(IAmmoInstance ammo);

        public IAmmo? LoadAmmo(IAmmo ammo);
        public IAmmo? ClearAmmo();
    }
}
