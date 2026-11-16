

using CharacterMainLib;

namespace InventoryCoreLib
{
    public class BootsInstance :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public BootsInstance(BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
        }
    }
}
