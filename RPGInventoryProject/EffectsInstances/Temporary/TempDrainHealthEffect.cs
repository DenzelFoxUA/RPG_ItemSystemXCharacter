

using CharacterMainLib;
using System;

namespace InventoryCoreLib
{
    public class TempDrainHealthEffect : TemporaryEffect<IAffectableActor, (string, uint)>
    {
        public TempDrainHealthEffect(uint power, float durationSec, string name = "Drain Health") 
            : base(("Health",power), durationSec, name)
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
            Console.WriteLine($"{Target} is sufferring {Name} by {PowerValue.Item2} pts " +
                $"{CurrentDuration} sec");
        }

        public override IAffect MakeMyCopy()
        {
            return new TempDrainHealthEffect(PowerValue.Item2, (uint)_duration, Name);
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

            _applyFunc = power => Target.DamageVital(power.Item1, power.Item2);
                
        }
    }
}
