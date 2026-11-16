

namespace CharacterMainLib
{
    public interface ICanUseEquipment<ItemType>
    {
        public abstract void Equip(ItemType item);

        /// <summary>
        /// Unequip item from a equipment slot
        /// </summary>
        /// <param name="slotName">SlotName from the system that youve mentioned when creating character</param>
        /// <returns></returns>
        public abstract ItemType? Unequip(string slotName);

        public ItemType? DropItem(ItemType item);
    }
}
