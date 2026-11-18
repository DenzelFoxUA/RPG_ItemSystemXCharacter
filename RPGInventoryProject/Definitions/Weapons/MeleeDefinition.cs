

using System.Collections.Generic;

namespace InventoryCoreLib
{
    public class MeleeDefinition : BaseWeapon
    {
        public MeleeDefinition(uint id, 
            string name, 
            string describtion, 
            double weight_kg,
            uint price,
            uint maxDurability,
            uint levelReq,
            bool isTwoHanded,
            AttackDefinition primary,
            AttackDefinition? secondary,
            bool isIndestructible,
            string iconPath,
            string miniaturePath) 
        {
            Id = id;
            Name = name;
            Descriprion = describtion;
            Weight = weight_kg;
            Price = price;
            IsStackable = isTwoHanded;
            MaxStackSize = 1;
            
            LevelRequired = levelReq;
            MaxDurability = maxDurability;
            IsIndestructible = isIndestructible;
            IsTwoHanded = false;
            EqupmentCategory = "Weapon";

            PrimaryAttack = primary;

            SecondaryAttack = secondary;
            
            Icon = iconPath;
            Miniature2D = miniaturePath;

        }
    }
}