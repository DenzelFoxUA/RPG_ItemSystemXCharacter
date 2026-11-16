using System;
using System.ComponentModel;

namespace CharacterMainLib
{
    public class AttributeStat : CharacterStat
    {
        public uint CurrentValue { get; protected set; }

        public AttributeStat(uint minValue, uint maxValue) : base(minValue, maxValue)
        {
            CurrentValue = minValue;
        }

        public override void Increase(uint value)
        {

            uint res = CurrentValue;

            bool isOk = CommonFunctions.Increase(ref res, value, MaxValue);

            if(isOk)
            {
                CurrentValue = res;
            }

            //if (CurrentValue >= MaxValue || value >= MaxValue - CurrentValue)
            //{
            //    CurrentValue = MaxValue;
            //}
            //else
            //{
            //    CurrentValue += value;
            //}
        }

        public override void Decrease(uint value)
        {
            uint res = CurrentValue;

            bool isOk = CommonFunctions.Decrease(ref res, value, MinValue);

            if (isOk)
            {
                CurrentValue = res;
            }

            //if (CurrentValue <= MinValue || value >= CurrentValue - MinValue)
            //{
            //    CurrentValue = MinValue;
            //}
            //else
            //{
            //    CurrentValue -= value;
            //}
        }

        public override string ToString()
        {
            return $"{CurrentValue}";
        }

        public override void Set(uint val)
        {
            if(val <= MaxValue && val >= MinValue)
            {
                CurrentValue = val;
            }
        }
    }
}
