using System;

namespace _Project.Scripts.Utils.MyAttribute
{
    
    /// <summary>
    /// My custom Button Attribute use for nonMethod
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MyButtonAttribute : Attribute
    {
        public string Label;

        public MyButtonAttribute(string label)
        {
            this.Label = label;
        }
    }

}