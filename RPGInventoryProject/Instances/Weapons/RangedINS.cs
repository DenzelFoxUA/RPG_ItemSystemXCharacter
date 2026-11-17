
using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public abstract class RangedINS :
        WeaponINS<IAffect, IAffectable>, IRanged
    {

        private Action<IOwnedEquipable>? _useFunction = null;
        private Func<string, IOwnedEquipable?>? _unequipFunction = null;
        private Action<IAmmo>? _clearAmmo = null;

        private RangedWeapon RangedWeapon => (RangedWeapon)Definition!;

        public IAmmo? CurrentAmmoInstance { get; protected set; }

        public uint CurrentAmmoCount => CurrentAmmoInstance!.GetStackCount();

        public RangedINS(RangedWeapon _weapon,
            params IAffect[] effects) :
            base(_weapon)
        {

        }

        public virtual IAmmo? LoadAmmo(IAmmo? ammo)
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

        public virtual IAmmo? ClearAmmo()
        {
            if (CurrentAmmoInstance is null)
                return null;

            IAmmo buffer = CurrentAmmoInstance;
            CurrentAmmoInstance = null;
            return buffer;
        }

        public override void Use()
        {
            SetTargetFunction();

            if (_useFunction is null || _unequipFunction is null)
                throw new NullReferenceException("Equip/Unequip funcs are not set");

            if (!IsEquipped)
            {
                _useFunction.Invoke(this);
            }
            else
            {
                _unequipFunction.Invoke(this.GetEquipSlotCategory());
            }
                
        }

        public override void Equip()
        {
            if (IsEquipped)
                return;

            IsEquipped = true;
        }

        public override void Unequip()
        {
            if (!IsEquipped)
                return;

            IsEquipped = false;

            CancelMyTarget();
        }

        public override void SetTargetFunction()
        {
            if (MyOwner is not null)
            {
                _useFunction = MyOwner.Equip;
                _unequipFunction = MyOwner.Unequip;
                _clearAmmo = MyOwner.PickUpItem;
            }
        }
    }
}
