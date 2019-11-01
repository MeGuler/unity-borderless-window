using System;
using System.ComponentModel;
using UnityEngine;

namespace Window.Deprecated
{
    [Obsolete("This usage has been deprecated.", false)]
    [Serializable]
    public class CursorData
    {
        public string name;
        public Texture2D image;
        public Vector2 offset;
    }
}