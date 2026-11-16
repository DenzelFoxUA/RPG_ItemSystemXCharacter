

namespace InventoryCoreLib
{
    public class ArmorDefinition : BaseArmor
    {
        public ArmorDefinition(uint id,
            string name,
            string describ,
            uint maxDefence,
            uint price,
            double weight,
            uint durability,
            string equipmentCategory,
            string icoRef,
            string miniature2DRef,
            bool isIndestructible = false)
        {
            Id = id;
            Name = name;
            Descriprion = describ;
            MaxDefence = maxDefence;
            IsIndestructible = isIndestructible;
            Price = price;
            Weight = weight;
            
            MaxDurability = durability;

            Icon = icoRef;
            Miniature2D = miniature2DRef;

            EqupmentCategory = equipmentCategory;
            IsStackable = false;

        }
    }
}
