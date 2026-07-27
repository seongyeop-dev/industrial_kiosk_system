using System;
using UnityEngine;

namespace Yeop
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute
    {
        public string Name { get; set; }

        public bool Persistent { get; set; }

        public bool Automatic { get; set; }

        public HideFlags HideFlags { get; set; } = HideFlags.None;
    }
}