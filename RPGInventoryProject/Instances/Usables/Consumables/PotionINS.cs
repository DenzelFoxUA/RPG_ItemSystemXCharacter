
using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class PotionINS :
         UsableItemInstance<IAffect, IAffectableCharacter>
    {

        private Action<IOwnedUsable>? _useFunction = null;

        public PotionINS(PotionDefinition potion, uint stackCount, 
            params IAffect[] effects) 
            : base(potion, stackCount, effects)
        {
            
        }

        public override void Use()
        {
            SetTargetFunction();

            if (CurrentStackCount > 0 && _useFunction is not null)
            {
                _useFunction.Invoke(this);
            }
        }

        public override void SetTargetFunction()
        {
            if (MyOwner is not null)
                _useFunction = MyOwner.UseConsumable;
            else
                throw new ArgumentNullException("Owner not set!");
        }
    }
}


