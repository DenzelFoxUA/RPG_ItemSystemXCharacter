
namespace InventoryCoreLib
{

    public class FoodDefinition: UsableItem
    {
        public FoodDefinition(
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
            MaxStackSize = maxStackSize;
            IsSelfApplycable = true;
            Icon = iconRef;
            Miniature2D = miniature2DRef;
        }

    }
}
