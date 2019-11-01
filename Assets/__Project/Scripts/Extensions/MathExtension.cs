using System;

namespace Window
{
    public static class MathExtension
    {
        public static bool IsBetween(float value, float minValue, float maxValue, bool includeValues)
        {
            if (minValue > maxValue)
            {
                var temp = maxValue;
                maxValue = minValue;
                minValue = temp;
            }

            if (includeValues)
            {
                if (value >= minValue && value <= maxValue)
                {
                    return true;
                }
            }
            else
            {
                if (value > minValue && value < maxValue)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsBetween(int value, int minValue, int maxValue, bool includeValues)
        {
            if (minValue > maxValue)
            {
                var temp = maxValue;
                maxValue = minValue;
                minValue = temp;
            }

            if (includeValues)
            {
                if (value >= minValue && value <= maxValue)
                {
                    return true;
                }
            }
            else
            {
                if (value > minValue && value < maxValue)
                {
                    return true;
                }
            }

            return false;
        }

        public static int GreatestCommonDivisor(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            // Pull out remainders.
            while (true)
            {
                var remainder = a % b;
                if (remainder == 0) return b;
                a = b;
                b = remainder;
            }
        }

        public static int LeastCommonMultiple(int a, int b)
        {
            return a * b / GreatestCommonDivisor(a, b);
        }
    }
}