using CharacterMainLib;

namespace InventoryCoreLib
{
    public class ShieldInstance :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public ShieldInstance(in BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
            
        }
    }
}
