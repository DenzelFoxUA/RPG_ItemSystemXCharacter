

using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class JewelryInstance : 
        EquipableInstance<IAffect, IAffectableCharacter>
    {
        private Action<IOwnedEquipable>? _useFunction = null;
        private Func<string, IOwnedEquipable?>? _unequipFunction = null;

        public JewelryInstance(EquipableItem definition,
            params IAffect[] effects)
            : base(definition, effects)
        {

        }

        public override void Equip()
        {
            if (IsEquipped)
                return;

            IsEquipped = true;

            foreach (var effect in Effects)
            {
                
                effect.Apply();
            }
        }

        public override void SetTargetFunction()
        {

            if (MyOwner is not null)
            {
                _useFunction = MyOwner.Equip;
                _unequipFunction = MyOwner.Unequip;
            }
        }

        public override void Unequip()
        {
            if (!IsEquipped)
                return;

            IsEquipped = false;

            foreach (var effect in Effects)
            {
                if(effect is IAffectConditionally<IAffectableCharacter> eff)
                {
                    //eff.SetTarget(MyOwner);
                    eff.Remove();
                }
            }

            CancelMyTarget();

        }

        public override void Update() { return; }

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
    }
}
