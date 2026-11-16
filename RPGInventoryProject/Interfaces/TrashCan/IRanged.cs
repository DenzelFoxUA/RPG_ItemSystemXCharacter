
namespace InventoryCoreLib
{
    public interface IRanged
    {
        public void Reload();
        //public bool CanUseAmmo(IAmmoInstance ammo);
        public AmmoInstance? SetAmmo(AmmoInstance ammo);
        public void ClearAmmo();
    }
}
