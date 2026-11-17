

using CharacterMainLib;

namespace InventoryCoreLib
{
    public class HelmetINS :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public HelmetINS(BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
            
        }
    }
}
