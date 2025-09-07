using System;

namespace _Project.Scripts.Effect
{
    public record EffectInstance
    {
        public IEffectTarget EffectTarget { get; }
        public IEffectData EffectData { get; }
        public IEffectLogic EffectLogic { get; }

        public EffectInstance(IEffectTarget effectTarget, IEffectData effectData, IEffectLogic effectLogic)
        {
            EffectTarget = effectTarget;
            EffectData = effectData;
            EffectLogic = effectLogic;
        }

        public void OnEffectActivate()
        {
            EffectData?.OnActivateEvent?.Invoke(EffectTarget);
        }

        public void StartEffect()
        {
            //can optz EffectTarget check by AOP.
            if (EffectTarget == null)
                throw new Exception("EffectTarget is null");
            EffectLogic?.OnStart(EffectTarget);
        }

        public void TriggerEffect()
        {
            if (EffectTarget == null || EffectData == null)
                throw new Exception("Effect Data or Effect Target is null");
            EffectLogic?.ExecuteTrigger(EffectTarget, EffectData);
        }

        public void StopEffect()
        {
            if (EffectTarget == null)
                throw new Exception("EffectTarget is null");
            EffectLogic?.OnStop(EffectTarget);
        }

        public void KillEffect()
        {
            if (true)
                StopEffect();
        }
    }
}