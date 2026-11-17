using CharacterMainLib;

namespace InventoryCoreLib
{
    public class GlovesINS :
        ArmorInstance<IAffect,
            IAffectableCharacter>
    {
        public GlovesINS(BaseArmor armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
        }
    }
}
