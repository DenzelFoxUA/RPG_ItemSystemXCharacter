
using System;

namespace InventoryCoreLib
{
    public class RangedWeapon : BaseWeapon
    {
        public string AmmoType { get; protected init; }

        public RangedWeapon(
            uint id,
            string name,
            string ammoType,
            string describtion,
            double weight_kg,
            uint price,
            uint maxDurability,
            uint levelReq,
            AttackDefinition primary,
            AttackDefinition? secondary,
            bool isIndestructible,
            bool isTwoHanded,
            string iconPath,
            string miniaturePath)
        {
            Id = id;
            Name = name;
            Descriprion = describtion;
            Weight = weight_kg;
            Price = price;
            IsStackable = false;
            MaxStackSize = 1;
            MaxDurability = maxDurability;
            EqupmentCategory = "Weapon";
            LevelRequired = levelReq;

            IsTwoHanded = isTwoHanded;
            IsIndestructible = isIndestructible;

            //attack set
            PrimaryAttack = primary;

            SecondaryAttack = secondary;

            AmmoType = ammoType;

            Icon = iconPath;
            Miniature2D = miniaturePath;

        }

        public override bool Equals(BaseItem? obj)
        {
            if (obj is null)
                return false;

            if (obj is not RangedWeapon other)
                return false;

            return base.Equals(obj) && AmmoType == other.AmmoType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), AmmoType);
        }
    }
}
