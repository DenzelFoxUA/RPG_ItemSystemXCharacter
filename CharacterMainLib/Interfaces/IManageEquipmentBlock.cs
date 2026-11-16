

namespace CharacterMainLib
{
    public interface IManageEquipmentBlock<T> where T : class, IEquipable
    {
        public bool TryEquip(ref T? item);
        public T? Unequip();
    }
}
