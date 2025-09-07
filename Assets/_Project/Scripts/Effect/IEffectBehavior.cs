using System;

namespace _Project.Scripts.Effect
{
    public interface IEffectBehavior
    {
        IEffectData EffectData { get; }
        IEffectLogic EffectLogic { get; }

        public EffectInstance PackageEffectInstance(IEffectTarget effectTarget)
        {
            if (EffectData == null || EffectLogic == null)
                throw new Exception("EffectData is null or EffectBehavior is null");
            return new EffectInstance(effectTarget, EffectData, EffectLogic);
        }
    }

    public static class EffectBehaviorExtensions
    {
        public static EffectInstance PackageEffectInstance(this IEffectBehavior behavior, IEffectTarget effectTarget)
        {
            return behavior.PackageEffectInstance(effectTarget);
        }
    }
}