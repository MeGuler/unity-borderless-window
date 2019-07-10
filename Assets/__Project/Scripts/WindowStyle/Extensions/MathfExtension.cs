using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtension
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
