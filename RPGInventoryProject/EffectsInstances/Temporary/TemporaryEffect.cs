using CharacterMainLib;
using System;
using System.Collections.Generic;

namespace InventoryCoreLib
{
    public abstract class TemporaryEffect<TTarget, TPower>
    : BaseEffect<TTarget, TPower>, IAffectTemporary<TTarget>
    where TTarget : class, IAffectable
    
    {
        protected Action<TPower>? _applyFunc;
        protected Action<TPower>? _restoreFunc;

        public bool IsExpired => CurrentDuration <= 0;

        protected float CurrentDuration { get; set; }
        protected float _duration; 

        protected TemporaryEffect(TPower power, float durationSec, string effectName)
        {
            if (durationSec <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(durationSec));

            _duration = durationSec;
            CurrentDuration = durationSec;
            PowerValue = power;
            Name = effectName;
        }

        public override void Apply()
        {
            if (IsExpired)
                return;

            if (_applyFunc is null || _restoreFunc is null)
                throw new InvalidOperationException("Functions of this effect are not set!");

            _applyFunc?.Invoke(PowerValue);
            _isApplied = true;
            Console.WriteLine($"{Target} is sufferring {Name} by {PowerValue} pts. \n" +
                $"{CurrentDuration} sec");
        }

        public override void Remove()
        {
            if (!_isApplied)
                return;

            _isApplied = false;
            _restoreFunc?.Invoke(PowerValue);
            
        }

        public virtual void Refresh()
        {
            CurrentDuration = _duration;
        }

        public virtual void Update(float deltaTime)
        {
            if (deltaTime <= 0.0f || IsExpired)
                return;

            Apply();

            CurrentDuration -= deltaTime;

            if (IsExpired)
            {
                Remove();
            }
                
        }

        public bool IsEffectExpired() => IsExpired;

        public override void SetTarget(TTarget? target)
        {
            if (_isApplied && !IsExpired)
                CurrentDuration = _duration;

            if(!ReferenceEquals(Target,target))
                Target = target;
        }

        public override bool Equals(BaseEffect<TTarget, TPower>? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is not TemporaryEffect<TTarget, TPower> temp)
                return false;

            return GetType() == temp.GetType()
                && Name == temp.Name
                && EqualityComparer<TPower>.Default.Equals(PowerValue, temp.PowerValue);
            //тут дефолтний .Net компаррер обирає найкращій тип для порівняння
        }

        public override bool Equals(object? obj) =>
            Equals(obj as TemporaryEffect<TTarget, TPower>);

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), Name, PowerValue);
        }
    }
}
