

using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class FoodINS : UsableItemInstance<IAffect,
            IAffectableCharacter>
    {

        private Action<IOwnedUsable>? _useFunction = null;

        public FoodINS(BaseItem item, uint stackCount, 
            params IAffect[]? effects) : base(item, stackCount, effects)
        {
        }

        public override void SetTargetFunction()
        {
            if (MyOwner is not null)
                _useFunction = MyOwner.UseConsumable;
            else
                throw new ArgumentNullException("Owner not set!");
        }

        public override void Use()
        {
            SetTargetFunction();

            if (CurrentStackCount > 0 && _useFunction is not null)
            {
                _useFunction.Invoke(this);
            }
        }
    }
}
