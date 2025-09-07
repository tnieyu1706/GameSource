using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Project.Scripts.Effect
{
    public class EffectDataMono : SerializedMonoBehaviour, IEffectData
    {
        [FormerlySerializedAs("effectName")] [SerializeField] private string effectBehaviorName;
        [SerializeField] private string id;
        [SerializeField] private EffectType effectType;
        [SerializeField] private float duration;
        [SerializeField] private float cooldown;
        [SerializeField] private bool canOverlap;
        
    
        public string DataBehaviorName => effectBehaviorName;
        public string Id => id;
        public EffectType EffectType => effectType;
        public float Duration => duration;
        public float Cooldown => cooldown;
        public bool CanOverlap => canOverlap;
        public UnityEvent<IEffectTarget> OnTriggerEvent { get; }
        public UnityEvent<IEffectTarget> OnActivateEvent { get; }
    }
}
