using System;

namespace _Project.Scripts.Utils
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UIShowLogAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.All)]
    public class NeedToSaveLoad : Attribute
    {
        
    }
}