
using CharacterMainLib;

namespace InventoryCoreLib
{
    public class BodyArmorInstance :
        ArmorInstance<IAffect,IAffectableCharacter>
    {

        public BodyArmorInstance(in ArmorDefinition armor, 
            params IAffect[] effects) 
            : base(armor, effects)
        {
           
        }

    }
}
