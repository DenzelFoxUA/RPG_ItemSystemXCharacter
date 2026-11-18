
using CharacterMainLib;

namespace InventoryCoreLib
{
    public interface IDemoCharacter : 
        ICollectEffects<IAffectableCharacter>,
        IAffectableCharacter, 
        ICanUseItems<IOwnedUsable>,
        ICanUseEquipment<IOwnedEquipable>,
        ICanUseTools
    {
        public void UseConsumable(IOwnedUsable? consumable);
        public double GetMaxCarryWeight();
        public void PickUpItem(IOwnedUsable? item);
    }
}
