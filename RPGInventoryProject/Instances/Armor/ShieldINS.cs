using CharacterMainLib;

namespace InventoryCoreLib
{
    public class ShieldINS :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public ShieldINS(in BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
            
        }
    }
}
