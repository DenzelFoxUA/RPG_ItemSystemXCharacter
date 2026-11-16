using CharacterMainLib;

namespace InventoryCoreLib
{
    public class GlovesInstance :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public GlovesInstance(BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
        }
    }
}
