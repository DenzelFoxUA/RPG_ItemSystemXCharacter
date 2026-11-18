
namespace InventoryCoreLib
{
    public class ToolDefinition : UsableItem
    {
        public uint UseValue { get; protected init; }
        //public uint MaxNumberOfUses { get; protected init; }

        public ToolDefinition(
             uint id,
             string name,
             uint useValue,
             string describtion,
             double weight_kg,
             uint price,
             uint stackCount,
             string iconRef,
             string miniature2DRef)
        {

            Id = id;
            Name = name;
            Descriprion = describtion;
            Weight = weight_kg;
            Price = price;
            UseValue = useValue;
            MaxStackSize = stackCount;
            IsSelfApplycable = false;
            Icon = iconRef;
            Miniature2D = miniature2DRef;
        }

    }
}
