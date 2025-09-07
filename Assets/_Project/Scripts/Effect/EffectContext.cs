using System;
using DG.Tweening;

namespace _Project.Scripts.Effect
{
    public class EffectContext : IEffectExecutor
    {
        public EffectInstance EffectInstance { get;}
        private Sequence _sequence;
        public IRemoveActivityFlow<EffectContext> RemoveActivityFlow { get; }
        public Sequence Sequence
        {
            get
            {
                if (_sequence == null)
                    _sequence = DOTween.Sequence();
                return _sequence;
            }
        }

        public EffectContext(EffectInstance effectInstance, IRemoveActivityFlow<EffectContext> removeActivityFlow)
        {
            EffectInstance = effectInstance;
            RemoveActivityFlow = removeActivityFlow;
        }

        public void Start()
        {
            EffectInstance.StartEffect();
            Sequence.IntervalAction(
                Trigger,
                EffectInstance.EffectData.Duration,
                EffectInstance.EffectLogic.Delay
            ).OnComplete(Stop).Play();

        }

        public void Trigger()
        {
            EffectInstance.TriggerEffect();
        }

        private void KillSequence()
        {
            _sequence.Kill();
        }

        public void Stop()
        {
            EffectInstance.StopEffect();
            KillSequence();
            RemoveActivityFlow.OnRemove(this);
        }

        public void Kill()
        {
            EffectInstance.KillEffect();
            KillSequence();
            RemoveActivityFlow.OnRemove(this);
        }
    }
}