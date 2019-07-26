using System;
using System.ComponentModel;

namespace Borderless
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
    }
    
    [Serializable]
    public class Vector4Int
    {
        [Description("Left")] public int x;
        [Description("Top")] public int y;
        [Description("Right")] public int z;
        [Description("Bottom")] public int w;
    }
}