using System;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public enum UIDisplayMode
    {
        Hide,
        Show,
        FirstShowAndHide
    }
    
    [Serializable]
    public struct UIDisplayModeData
    {
        public GameObject displayObj;
        public UIDisplayMode displayMode;
    }
}