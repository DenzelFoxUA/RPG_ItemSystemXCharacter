
namespace CharacterMainLib
{

    /// <summary>
    /// Interface of all equipment section
    /// </summary>
    public interface IEquipable : IUsable
    {
        public void Equip();
        public void Unequip();
        public string GetEquipSlotCategory();
    }
}
