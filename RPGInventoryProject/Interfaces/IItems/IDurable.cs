
using CharacterMainLib;

namespace InventoryCoreLib
{
    public interface IDurable : IAffectable
    {
        public double GetDurabilityCoefficient();

        public uint GetCurrentDurability();

        public void Repair(uint points);

        public void TakeDamage(uint damage);

    }
}
