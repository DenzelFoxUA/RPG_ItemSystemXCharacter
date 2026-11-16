

using CharacterMainLib;

namespace InventoryCoreLib
{
    public class HelmetInstance :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public HelmetInstance(BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
            
        }
    }
}
