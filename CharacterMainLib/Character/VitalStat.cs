
namespace CharacterMainLib
{
    public class VitalStat : CharacterStat
    {

        public uint CurrentMax { get; protected set; }
        public uint CurrentValue { get; protected set; }


        public VitalStat(uint startingMaxValue, uint peakValue) : base(0, peakValue)
        {
            CurrentMax = startingMaxValue;
            CurrentValue = CurrentMax;
        }

        public override string ToString()
        {
            return $"{CurrentValue}/{CurrentMax}";
        }

        //methods

        /// <summary>
        /// Restores to CurrentMax
        /// </summary>
        /// <param name="value">Amount of health</param>
        public override void Increase(uint value)
        {
            uint res = CurrentValue;

            bool isOk = CommonFunctions.Increase(ref res, value, CurrentMax);

            if (isOk)
            {
                CurrentValue = res;
            }

            //if (CurrentValue >= CurrentMax || value >= CurrentMax - CurrentValue)
            //{
            //    CurrentValue = CurrentMax;
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

        /// <summary>
        /// Increasing CurrentMax until PeakValue
        /// </summary>
        public void IncreaseMax(uint value)
        {

            uint res = CurrentMax;

            bool isOk = CommonFunctions.Decrease(ref res, value, MaxValue);

            if (isOk)
            {
                CurrentMax = res;
            }

            //if (CurrentMax >= MaxValue || valueIncrement >= MaxValue - CurrentMax)
            //{
            //    CurrentMax = MaxValue;
            //}
            //else
            //{
            //    CurrentMax += valueIncrement;
            //}
        }

        public void DecreaseMax(uint value)
        {

            uint res = CurrentMax;

            uint min = 1;

            bool isOk = CommonFunctions.Decrease(ref res, value, min);

            if (isOk)
            {
                CurrentMax = res;
            }


            //if (CurrentMax <= bareMinimum || valueDecrement >= CurrentMax - bareMinimum)
            //{
            //    CurrentMax = bareMinimum;
            //}
            //else
            //{
            //    CurrentMax -= valueDecrement;
            //}
        }

        /// <summary>
        /// Immidietly sets CurrentMax to value in range 0 < val <= MaxVal
        /// </summary>
        public override void Set(uint val)
        {
            if (val <= MaxValue && val > MinValue)
            {
                CurrentMax = val;
            }
        }

    }
}
