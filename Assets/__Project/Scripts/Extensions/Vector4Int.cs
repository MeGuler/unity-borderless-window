using System;
using System.ComponentModel;

namespace Window.Structures
{
    [Serializable]
    public class Vector4Int
    {
        [Description("Left")] public int x;
        [Description("Top")] public int y;
        [Description("Right")] public int z;
        [Description("Bottom")] public int w;
    }
}