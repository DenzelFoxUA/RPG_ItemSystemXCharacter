
using CharacterMainLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryCoreLib
{
    public class RangedINS :
        WeaponINS<IAffect, IAffectable>, IRanged
    {

        private Action<IOwnedEquipable>? _useFunction = null;

        private Func<string, IOwnedEquipable?>? _unequipFunction = null;

        private RangedWeapon RangedWeapon => (RangedWeapon)Definition!;

        public IAmmo? CurrentAmmoInstance { get; protected set; }

        public uint CurrentAmmoCount => CurrentAmmoInstance!.CurrentStackCount;

        public RangedINS(RangedWeapon _weapon,
            params IAffect[] effects) :
            base(_weapon)
        {

        }

        public virtual void SetAmmo(IAmmo? ammo)
        {
            if (ammo is null)
                return;

            CurrentAmmoInstance = ammo;
        }

        public virtual void ClearAmmo()
        {
            CurrentAmmoInstance = null;
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

            if(CurrentAmmoInstance is not null)
            {
                CurrentAmmoInstance!.Use();
            }
            
            CancelMyTarget();
        }

        public override void SetTargetFunction()
        {
            if (MyOwner is not null)
            {
                _useFunction = MyOwner.Equip;
                _unequipFunction = MyOwner.Unequip;
            }
        }

        public override AttackComplex PrimaryAttack(IAffectable? target)
        {
            if (Weapon.PrimaryAttack == null)
                throw new ObjectNotInitializedException(nameof(Weapon.PrimaryAttack));

            if(CurrentAmmoInstance is null)
            {
                return null!;
            }

            if (RangedWeapon.AmmoType != CurrentAmmoInstance!.GetAmmoType())
            {
                throw new CannotUseItemException(nameof(CurrentAmmoInstance));
            }

            var dist = Weapon.PrimaryAttack.AttackRange;

            var completeEffects = Effects.ToList();

            if (CurrentAmmoInstance is not null)
            {
                CurrentAmmoInstance.RemoveFromStack(1);

                completeEffects = GetFullEffectsList();
            }

            return new AttackComplex(CurrentPrimaryDamage, dist, completeEffects);
        }

        public override AttackComplex? SecondaryAttack(IAffectable? target)
        {
            if (Weapon.SecondaryAttack is null)
            {
                return null;
            }

            if (RangedWeapon.AmmoType != CurrentAmmoInstance!.GetAmmoType())
            {
                throw new CannotUseItemException(nameof(CurrentAmmoInstance));
            }

            var dist = Weapon.PrimaryAttack.AttackRange;

            var completeEffects = Effects.ToList();

            if (CurrentAmmoInstance is not null)
            {
                CurrentAmmoInstance.RemoveFromStack(1);

                completeEffects = GetFullEffectsList();
            }
                
            return new AttackComplex(CurrentSecondaryDamage, dist, completeEffects);
        }

        public override void SetMyTarget(IAffectable? target)
        {
            if (ReferenceEquals(target, MyOwner))
                return;

            Target = target;

            if(CurrentAmmoInstance is null)
            {
                Console.WriteLine("Cannot shot without arrows");
                return;
            }
            
            CurrentAmmoInstance!.SetMyTarget(Target);
        }

        public string GetAmmoType() => RangedWeapon.AmmoType;

        protected List<IAffect> GetFullEffectsList()
        {
            var completeEffects = Effects.ToList();

            if (CurrentAmmoInstance!.GetEffects() is not null)
            {
                foreach (var effect in CurrentAmmoInstance.GetEffects()!)
                {
                    completeEffects.Add(effect);
                }
            }

            return completeEffects;
        }

    }
}
