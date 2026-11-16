

using CharacterMainLib;
using System;
using System.Numerics;

namespace InventoryCoreLib
{
    public class BuffAttributeEffect
        : BaseEffect<IAffectableCharacter, uint>, 
        IAffectConditionally<IAffectableCharacter>
    {
        private string _attributeName;

        protected Action<string, uint>? _applyFunc;
        protected Action<string, uint>? _restoreFunc;

        protected bool _isRemoved = true;

        public BuffAttributeEffect(string attributeName, uint power, string effName)
        {
            _attributeName = attributeName;
            PowerValue = power;
            Name = effName;
        }

        public override void Apply()
        {
            if (_isApplied)
                return;

            if (_applyFunc is null)
                throw new ObjectNotInitializedException(nameof(_applyFunc));

            if(_restoreFunc is null)
                throw new ObjectNotInitializedException(nameof(_restoreFunc));

            _applyFunc?.Invoke(_attributeName, PowerValue);
            _isApplied = true;
            _isRemoved = false;
        }

        public override void Remove()
        {
            if (_isRemoved)
                return;

            _restoreFunc?.Invoke(_attributeName, PowerValue);
            _isApplied = false;
            _isRemoved = true;
        }

        public override IAffect MakeMyCopy()
        {
            return new BuffAttributeEffect(_attributeName, PowerValue, Name);
        }

        public override void SetTarget(IAffectableCharacter? target)
        {
            base.SetTarget(target);

            if (Target is null)
                return;

            _applyFunc = Target.IncreaseAttribute;
            _restoreFunc = Target.DecreaseAttribute;

        }
    }
}
