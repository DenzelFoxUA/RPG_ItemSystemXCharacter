

using CharacterMainLib;

namespace InventoryCoreLib
{
    public class BootsINS :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public BootsINS(BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
        }
    }
}
