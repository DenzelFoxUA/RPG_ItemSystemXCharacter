using CharacterMainLib;

namespace InventoryCoreLib
{
    public interface ICanUseTools
    {
        public void Repair(IOwnedUsable tool, IAffectable? target);
    }
}
