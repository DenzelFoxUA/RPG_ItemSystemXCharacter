using System.Numerics;
namespace CharacterMainLib
{
    public static class CommonFunctions
    {
        public static T? Move<T>(ref T? source) where T : class
        {
            T? result = source;
            source = null;
            return result;
        }

        public static void Swap<T>(ref T? a, ref T? b) where T : class
        {
            (a, b) = (b, a);
        }

        public static bool Increase<T>(ref T current, T amount, T baseMax)
            where T : INumber<T>, IMinMaxValue<T>
        {
            if (amount <= T.Zero)
                return false;

            if (current >= baseMax)
            {
                current = baseMax;
                return false;
            }

            if (amount >= baseMax - current)
                current = baseMax;
            else
                current += amount;

            return true;
        }

        public static bool Decrease<T>(ref T current, T amount, T baseMin)
            where T : INumber<T>
        {
            if (amount <= T.Zero)
                return false;

            if (current <= baseMin)
            {
                current = baseMin;
                return false;
            }

            if (amount >= current - baseMin)
                current = baseMin;
            else
                current -= amount;

            return true;
        }
    }
}
