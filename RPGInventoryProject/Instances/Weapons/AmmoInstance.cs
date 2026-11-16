using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class AmmoInstance : UsableItemInstance<IAffect,IAffectable>
    {


        public AmmoInstance(AmmoDefinition definition, 
            uint count, 
            params IAffect[] effects) 
            : base(definition,count, effects)
        {
            
        }

        public override void SetTargetFunction()
        {
            throw new NotImplementedException();
        }

        public override void Use()
        {
            throw new NotImplementedException();
        }
    }
}
