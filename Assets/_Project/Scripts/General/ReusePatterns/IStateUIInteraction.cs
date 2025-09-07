using System;

namespace _Project.Scripts.General.ReusePatterns
{
    public interface IStateUIInteraction
    {
        public Action OnUIInitialized { get; }
        public Action OnUIClosed { get; }
    }
}