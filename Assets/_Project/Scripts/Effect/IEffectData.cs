using _Project.Scripts.Item;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Effect
{
    public interface IEffectData
    {
        EffectType EffectType { get; }
        float Duration { get; }
        float Cooldown { get; }
        bool CanOverlap { get; }
        UnityEvent<IEffectTarget> OnTriggerEvent { get; }
        UnityEvent<IEffectTarget> OnActivateEvent { get; }
        
    }
}