
using System;

namespace InventoryCoreLib
{
    public class AmmoDefinition : EquipableItem
    {
        public string AmmoType { get; private init; }

        public AmmoDefinition(
             uint id,
             string type,
             string name,
             string describtion,
             double weight_kg,
             uint price,
             uint maxStackSize,
             string iconRef,
             string miniature2DRef)
        {

            Id = id;
            AmmoType = type;
            Name = name;
            Descriprion = describtion;
            Weight = weight_kg;
            Price = price;
            IsStackable = true;
            MaxStackSize = maxStackSize;
            Icon = iconRef;
            Miniature2D = miniature2DRef;

            EqupmentCategory = "Ammo";
        }

        public override bool Equals(BaseItem? obj)
        {
            if (obj is null)
                return false;

            if (obj is not AmmoDefinition other)
                return false;

            return base.Equals(obj) &&
                AmmoType == other.AmmoType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), AmmoType);
        }

        public override string ToString()
        {
            return base.ToString() +
                $"Type: {AmmoType}\n";
        }
    }
}
