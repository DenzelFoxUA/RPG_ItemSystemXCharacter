using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class AmmoINS : EquipableInstance<IAffect,IAffectable>, IAmmo
    {
        private Action<IAmmo>? _useFunction = null;
        private Func<string,IOwnedEquipable?>? _clearFunction = null;

        private AmmoDefinition Ammo => (AmmoDefinition)Definition!;

        protected uint _currentStackCount = 0;

        public uint CurrentStackCount => _currentStackCount;

        public AmmoINS(AmmoDefinition definition, uint count, params IAffect[] effects) 
            : base(definition, effects)
        {
            _currentStackCount = Math.Clamp(count, 1u, Definition!.MaxStackSize);
        }

        public override void SetTargetFunction()
        {
            if (MyOwner is not null)
            {
                _useFunction = MyOwner.Equip;
                _clearFunction = MyOwner.Unequip;
            }
        }

        public override void Use()
        {
            SetTargetFunction();

            if (_useFunction is null || _clearFunction is null)
                throw new NullReferenceException("Load/Clear funcs are not set");

            if (!IsEquipped)
                _useFunction.Invoke(this);
            else
                _clearFunction.Invoke(this.GetEquipSlotCategory());
        }

        public EquipableItem? GetDefinition() => Definition as AmmoDefinition;

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

        public override void Update()
        {
            if (CurrentStackCount == 0)
                _clearFunction!.Invoke(this.GetEquipSlotCategory());
        }

        public override void SetMyTarget(IAffectable? target)
        {
            if (ReferenceEquals(target, MyOwner))
                return;

            _target = target;
        }

        public string GetAmmoType()
        {
            return Ammo.AmmoType;
        }


        #region [ STACK OPERATIONS ]

        public virtual uint AddToStack(uint count)
        {
            if (Definition is null)
                throw new NullReferenceException("Definition of item not set");

            if (!Definition.IsStackable)
                return count;

            if (CurrentStackCount >= Definition.MaxStackSize)
            {
                return count;
            }
            else if (count >= Definition.MaxStackSize - CurrentStackCount)
            {
                uint difference = Definition.MaxStackSize - CurrentStackCount;
                _currentStackCount = Definition.MaxStackSize;
                return count - difference;
            }
            else
            {
                _currentStackCount += count;
                return 0;
            }
        }

        public virtual uint RemoveFromStack(uint count)
        {
            if (Definition is null)
                throw new NullReferenceException("Definition of item not set");

            if (!Definition.IsStackable)
                return 0;

            uint toRemove = Math.Min(count, CurrentStackCount);

            _currentStackCount -= toRemove;

            return count - toRemove;
        }

        #endregion
    }
}
