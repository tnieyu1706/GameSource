using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Project.Scripts.Effect
{
    public interface IEffectLogic
    {
        EffectType EffectType { get; }
        float Delay { get; }
        Sprite Icon { get; }
        GameObject VfxPrefab { get; }
        AudioClip SfxSource { get; }
        
        void OnStart(IEffectTarget effectTarget);
        void OnTrigger(IEffectTarget effectTarget);
        void OnStop(IEffectTarget effectTarget);

        Sequence EffectSequence { get; protected set; }

        public void ExecuteTrigger(IEffectTarget effectTarget, IEffectData effectData)
        {
            effectData.OnTriggerEvent?.Invoke(effectTarget);
            OnTrigger(effectTarget);
        }

        public void ActivateEffect(IEffectTarget effectTarget, IEffectData effectData)
        {
            effectData.OnActivateEvent?.Invoke(effectTarget);
            OnStart(effectTarget);
            
            EffectSequence = DOTween.Sequence()
                .IntervalAction(
                    () => ExecuteTrigger(effectTarget, effectData),
                    effectData.Duration,
                    Delay
                ).OnComplete(() => OnStop(effectTarget))
                .Play();
            
        }

        public void KillEffect()
        {
            if (EffectSequence != null && EffectSequence.IsActive())
            {
                EffectSequence.Kill();
            }
        }

        public void PauseEffect()
        {
            if (EffectSequence != null && EffectSequence.IsPlaying())
            {
                EffectSequence.Pause();
            }
        }

        public void ResumeEffect()
        {
            if (EffectSequence != null && !EffectSequence.IsPlaying())
            {
                EffectSequence.Play();
            }
        }
    }
    
    public static class EffectLogicExtensions
    {
        public static void ExecuteTrigger(this IEffectLogic effectLogic, IEffectTarget effectTarget, IEffectData effectData)
        {
            effectLogic.ExecuteTrigger(effectTarget, effectData);
        }
        
        public static void ActivateEffect(this IEffectLogic effectLogic, IEffectTarget effectTarget, IEffectData effectData)
        {
            effectLogic.ActivateEffect(effectTarget, effectData);
        }
    
        public static void KillEffect(this IEffectLogic effectLogic)
        {
            effectLogic.KillEffect();
        }
    
        public static void PauseEffect(this IEffectLogic effectLogic)
        {
            effectLogic.PauseEffect();
        }
    
        public static void ResumeEffect(this IEffectLogic effectLogic)
        {
            effectLogic.ResumeEffect();
        }
        
    }
    
    [CreateAssetMenu(fileName = "EffectBehavior", menuName = "Scriptable Objects/EffectSystem/EffectLogic")]
    public class EffectLogic: SerializedScriptableObject, IEffectLogic
    {
        [FormerlySerializedAs("effectBehaviourName")] [SerializeField] private string effectBehaviourBehaviorName;
        [SerializeField] private string id;
        [SerializeField] private EffectType effectType;
        [SerializeField] private float delay;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject vfxPrefab;
        [SerializeField] private AudioClip sfxSource;
        private Sequence _effectSequence;
        public string DataBehaviorName => effectBehaviourBehaviorName;
        public string Id => id;
        public EffectType EffectType => effectType;
        public float Delay => delay;
        public Sprite Icon => icon;
        public GameObject VfxPrefab => vfxPrefab;
        public AudioClip SfxSource => sfxSource;
        Sequence IEffectLogic.EffectSequence
        {
            get => _effectSequence;
            set => _effectSequence = value;
        }
        public void OnStart(IEffectTarget effectTarget)
        {
            Debug.Log($"On Start: {DataBehaviorName}");
        }

        public void OnTrigger(IEffectTarget effectTarget)
        {
            Debug.Log($"On Trigger: {DataBehaviorName}");
        }

        public void OnStop(IEffectTarget effectTarget)
        {
            Debug.Log($"On Stop: {DataBehaviorName}");
        }
        
    }
}