
namespace InventoryCoreLib
{
    public interface IRanged
    {
        //public void Reload();
        //public bool CanUseAmmo(IAmmoInstance ammo);

        public string GetAmmoType();

        public void SetAmmo(IAmmo? ammo);
        public void ClearAmmo();
    }
}
