
using CharacterMainLib;

namespace InventoryCoreLib
{
    public class RangedInstance :
        WeaponInstance<IAffect, IAffectable>, IRanged
    {

        public RangedWeapon RangedWeapon => (RangedWeapon)Definition!;

        public AmmoInstance? CurrentAmmoInstance { get; protected set; }

        public uint CurrentAmmoCount => CurrentAmmoInstance!.CurrentStackCount;

        public RangedInstance(RangedWeapon _weapon,
            params IAffect[] effects) :
            base(_weapon)
        {

        }

        public virtual void Reload()
        {

        }

        //public abstract bool CanUseAmmo(IAmmoInstance ammo);

        public virtual AmmoInstance? SetAmmo(AmmoInstance? ammo)
        {
            if (ammo is null)
                return null;
            if(CurrentAmmoInstance is not null)
            {
                (CurrentAmmoInstance, ammo) = (ammo, CurrentAmmoInstance);
            }
            else
            {
                CurrentAmmoInstance = ammo;
                ammo = null;
            }

            return ammo;
        }

        public virtual void ClearAmmo()
        {

        }

        public override void Use()
        {
            throw new System.NotImplementedException();
        }

        public override void Equip()
        {
            throw new System.NotImplementedException();
        }

        public override void Unequip()
        {
            throw new System.NotImplementedException();
        }

        public override void SetTargetFunction()
        {
            throw new System.NotImplementedException();
        }
    }
}
