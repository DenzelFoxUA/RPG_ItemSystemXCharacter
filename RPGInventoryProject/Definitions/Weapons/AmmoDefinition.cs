using System;

namespace InventoryCoreLib
{
    public class AmmoDefinition : EquipableItem
    {
        public AmmoDefinition(
             uint id,
             string name,
             string describtion,
             double weight_kg,
             uint price,
             uint maxStackSize,
             string iconRef,
             string miniature2DRef)
        {

            Id = id;
            Name = name;
            Descriprion = describtion;
            Weight = weight_kg;
            Price = price;
            IsStackable = true;
            MaxStackSize = maxStackSize;
            Icon = iconRef;
            Miniature2D = miniature2DRef;
        }
    }
}
