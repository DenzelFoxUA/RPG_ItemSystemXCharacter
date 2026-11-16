

using CharacterMainLib;

namespace InventoryCoreLib
{
    public interface IOwnedEquipable : IOwnedUsable, IEquipable
    {
        public EquipableItem? GetDefinition();
        public bool IsItemEquipped();
        public void Update();
    }

}
