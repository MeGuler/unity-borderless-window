using System;
using System.ComponentModel;
using UnityEngine;

namespace Borderless.Api.Structures
{
    [Serializable]
    public class CursorData
    {
        public string name;
        public Texture2D image;
        public Vector2 offset;
    }

    
}