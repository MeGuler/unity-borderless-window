using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borderless.Flags
{
    public enum MonitorFlag : uint
    {
        /// <summary>Returns NULL.</summary>
        DefaultToNull = 0,

        /// <summary>Returns a handle to the primary display monitor.</summary>
        DefaultToPrimary = 1,

        /// <summary>Returns a handle to the display monitor that is nearest to the window.</summary>
        DefaultToNearest = 2
    }
}