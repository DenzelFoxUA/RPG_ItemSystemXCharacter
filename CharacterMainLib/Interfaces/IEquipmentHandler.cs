
namespace CharacterMainLib
{
    public interface IEquipmentHandler<T>
    {
        public bool TryEquipIntoSlot(ref T? item);

        public T? UnequipFromSlot(string slotName);

        public void Update(float deltaTime);
        public string ItemInfo(string slotName);

        public IEquipable? GetInstanse(string blockName);

        public string ShowEquipment();
    }
}
