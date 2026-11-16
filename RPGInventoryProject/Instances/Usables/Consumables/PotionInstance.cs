
using CharacterMainLib;
using System;
using System.Linq;
using System.Collections.Generic;

namespace InventoryCoreLib
{
    public class PotionInstance :
         UsableItemInstance<IAffect, 
            IAffectableCharacter>
    {

        private Action<IOwnedUsable>? _useFunction = null;

        public PotionInstance(PotionDefinition potion, uint stackCount, 
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


