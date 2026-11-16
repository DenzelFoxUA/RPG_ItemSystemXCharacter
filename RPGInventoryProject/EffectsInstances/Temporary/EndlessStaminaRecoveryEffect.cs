
using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class EndlessStaminaRecoveryEffect : TemporaryEffect<IAffectableActor, (string, uint)>
    {
        public EndlessStaminaRecoveryEffect(uint power, float durationSec, string effectName) 
            : base(("Stamina",power), durationSec, effectName)
        {

        }

        public override void Apply()
        {
            if (IsExpired)
                return;

            if (_applyFunc is null)
                throw new InvalidOperationException("Functions of this effect are not set!");

            _applyFunc?.Invoke(PowerValue);
            _isApplied = true;
        }

        public override IAffect MakeMyCopy()
        {
            return new EndlessStaminaRecoveryEffect(PowerValue.Item2, _duration, Name);
        }

        public override void Update(float deltaTime)
        {
            if (deltaTime <= 0.0f || IsExpired)
                return;

            Apply();

            if (IsExpired)
            {
                Remove();
            }

        }

        public override void Remove()
        {
            if (!_isApplied)
                return;

            _isApplied = false;
        }

        public override void SetTarget(IAffectableActor? target)
        {
            base.SetTarget(target);

            if (Target is null)
                return;

            _applyFunc = power => Target.RestoreVital(power.Item1, power.Item2);

        }
    }
}
