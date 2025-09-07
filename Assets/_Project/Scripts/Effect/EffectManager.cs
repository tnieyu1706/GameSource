using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace _Project.Scripts.Effect
{
    public class EffectManager
    {
        private readonly LinkedList<EffectContext> _contexts = new LinkedList<EffectContext>();
        public LinkedList<EffectContext> Collection => _contexts;
        public IRemoveActivityFlow<EffectManager> RemoveActivityFlow { get; }

        public EffectManager(IRemoveActivityFlow<EffectManager> removeActivityFlow)
        {
            RemoveActivityFlow = removeActivityFlow;
        }

        public bool Remove(EffectContext obj)
        {
            bool isRemove = _contexts.Remove(obj);
            if (Collection.Count == 0)
            {
                RemoveActivityFlow.OnRemove(this);
            }

            return isRemove;
        }

        public void AppendInstance(EffectInstance effectInstance)
        {
            EffectContext effectContext = new EffectContext(effectInstance, new EffectManagerRemoveActivityFlow(this));

            _contexts.AddLast(effectContext);

            effectContext.Start();
        }

        public IEnumerable<EffectContext> GetContextsByInstance(EffectInstance effectInstance)
        {
            return Collection.Where(c => c.EffectInstance == effectInstance);
        }

        private void KillSafely(EffectContext effectContext)
        {
            effectContext.Kill();
            if (Collection.Contains(effectContext))
                Remove(effectContext);
        }

        public bool KillAllInstance(EffectInstance effectInstance)
        {
            IEnumerable<EffectContext> effectContexts = GetContextsByInstance(effectInstance).ToList();
            if (!effectContexts.Any())
                return false;

            effectContexts.ForEach((c) => KillSafely(c));
            return true;
        }

        public bool KillFirstInstance(EffectInstance effectInstance)
        {
            EffectContext firstContext = GetContextsByInstance(effectInstance).FirstOrDefault();
            if (firstContext == null)
                return false;
            
            KillSafely(firstContext);
            return true;
        }

        public bool KillLastInstance(EffectInstance effectInstance)
        {
            EffectContext lastContext = GetContextsByInstance(effectInstance).LastOrDefault();
            if (lastContext == null)
                return false;
            
            KillSafely(lastContext);
            return true;
        }

        public void Kill()
        {
            foreach (EffectContext context in Collection)
            {
                KillSafely(context);
            }
        }

        private class EffectManagerRemoveActivityFlow : IRemoveActivityFlow<EffectContext>
        {
            private EffectManager _manager;

            public EffectManagerRemoveActivityFlow(EffectManager manager)
            {
                _manager = manager;
            }

            public void OnRemove(EffectContext obj)
            {
                _manager.Remove(obj);
            }
        }
    }
}