
using CharacterMainLib;

namespace InventoryCoreLib
{
    public class BodyArmorINS :
        ArmorInstance<IAffect,IAffectableCharacter>
    {

        public BodyArmorINS(in ArmorDefinition armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
           
        }

    }
}
