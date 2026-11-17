
using CharacterMainLib;
using System;


namespace InventoryCoreLib
{
    public class ShortBladeINS
        : WeaponINS<IAffect, 
            IAffectableCharacter>
    {
        private ShortBladeDefinition Dagger => (ShortBladeDefinition)Definition!;

        private Action<IOwnedEquipable>? _useFunction = null;
        private Func<string, IOwnedEquipable?>? _unequipFunction = null;

        public ShortBladeINS(ShortBladeDefinition daggerItem, 
            params IAffect[]? effects) : base(daggerItem, effects)
        {

        }

        public override void Use()
        {
            SetTargetFunction();

            if (_useFunction is null || _unequipFunction is null)
                throw new NullReferenceException("Equip/Unequip funcs are not set");

            if (!IsEquipped)
                _useFunction.Invoke(this);
            else
                _unequipFunction.Invoke(this.GetEquipSlotCategory());
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
            }
        }

    }
}
