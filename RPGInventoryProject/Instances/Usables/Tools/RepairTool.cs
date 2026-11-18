
using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class RepairTool : 
        UsableItemInstance<IAffectImmediately<IDurable>, IDurable>,
        IRepair
    {
        private Action<IOwnedUsable,IDurable>? _useFunction = null;

        private ToolDefinition Tool => (ToolDefinition)Definition!;

        public RepairTool(ToolDefinition item, uint stackCount) 
                : base(item, stackCount)
        {
        }

        public override void SetTargetFunction()
        {
            if (MyOwner is not null)
                _useFunction = MyOwner.Repair;
            else
                throw new ArgumentNullException("Owner not set!");
        }

        public override void Use()
        {
            SetTargetFunction();

            if (CurrentStackCount > 0 && _useFunction is not null)
            {
                if(MyTarget is null)
                {
                    Console.WriteLine($"No target selected");
                    return;
                }
                
                _useFunction.Invoke(this, MyTarget);
            }
        }

        public override void SetMyTarget(IAffectable? target)
        {
            if (target is not IDurable durableItem)
            {
                throw new CannotUseItemException("Repair Tool");
            }

            _target = durableItem;
        }

        public void RepairItem()
        {
            if (_target is null)
                return;

            _target.Repair(Tool.UseValue);
            
        }
    }
}
