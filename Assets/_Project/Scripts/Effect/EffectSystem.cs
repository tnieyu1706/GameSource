using System.Collections.Generic;
using JetBrains.Annotations;

namespace _Project.Scripts.Effect
{
    public class EffectSystem
    {
        private static EffectSystem _instance;
        public static EffectSystem Instance => _instance ?? (_instance = new EffectSystem());

        private Dictionary<IEffectTarget, EffectManager> _targets = new Dictionary<IEffectTarget, EffectManager>();

        public Dictionary<IEffectTarget, EffectManager> Collection
        {
            get => _targets;
            protected set => _targets = value ?? new Dictionary<IEffectTarget, EffectManager>();
        }

        public void RegistryEffect(IEffectTarget target, EffectInstance effectInstance)
        {
            EffectManager manager;
            if (!Collection.ContainsKey(target))
            {
                manager = new EffectManager(new EffectSystemRemoveActivityFlow(this, target));
                _targets.Add(target, manager);
            }
            else
            {
                manager = Collection[target];
            }
            
            manager.AppendInstance(effectInstance);
        }

        public bool UnRegistryEffect(IEffectTarget target, [CanBeNull] EffectInstance effectInstance=null)
        {
            if (!Collection.ContainsKey(target))
                return false;
            
            EffectManager manager = Collection[target];
            if (effectInstance == null)
            {
                manager.Kill();
                if (Collection.ContainsKey(target))
                    _targets.Remove(target);
                return true;
            }
            
            return manager.KillLastInstance(effectInstance);
        }
        
        private class EffectSystemRemoveActivityFlow : IRemoveActivityFlow<EffectManager>
        {
            private EffectSystem _effectSystem;
            private IEffectTarget _effectTarget;
        
            public EffectSystemRemoveActivityFlow(EffectSystem effectSystem, IEffectTarget effectTarget)
            {
                _effectSystem = effectSystem;
                _effectTarget = effectTarget;
            }
            
            public void OnRemove(EffectManager obj)
            {
                _effectSystem.Collection.Remove(_effectTarget);
            }
        }
    }
}