using System;
using System.Collections.Generic;
using _Project.Scripts.Config;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Effect
{
    /// <summary>
    /// EffectManager using Batching system but now Architecture of EffectInstance
    /// not response Logic of Bathing when append after start
    /// </summary>
    public class EffectManagerBatching
    {
        private LinkedList<EffectInstance> _instances = new LinkedList<EffectInstance>();

        public LinkedList<EffectInstance> Collection
        {
            get => _instances;
            protected set => _instances = value;
        }

        private Sequence _sequence;

        private void InitSequence()
        {
            if (_sequence == null)
                _sequence = DOTween.Sequence().SetAutoKill(false).Play();
        }
        
        protected float CalculateBonusTime(float newInstanceTime)
        {
            float lastRemaining = _sequence.Duration() - _sequence.Elapsed();
            return newInstanceTime - lastRemaining;
        }

        private int count = 0;

        public void Append(EffectInstance obj)
        {
            ++count;
            InitSequence();
            Start();
            
            this.Append(obj);
            obj.OnEffectActivate();
            obj.StartEffect();

            float bonusTime = CalculateBonusTime(obj.EffectData.Duration);
            if (bonusTime > 0)
                AppendExecutor(bonusTime);

            float removeTime = _sequence.Elapsed() + obj.EffectData.Duration;
            if (removeTime >= _sequence.Duration())
            {
                Debug.Log("remove time is too long");
            }
            _sequence.InsertCallback(
                removeTime,
                () => StopInstance(obj)
            );
        }

        public bool Remove(EffectInstance obj)
        {
            if (!Collection.Contains(obj))
                return false;

            _instances.Remove(obj);
            Debug.Log($"Collection count: {Collection.Count}");
            if (Collection.Count == 0)
                Stop();
            return true;
        }

        public void StopInstance(EffectInstance obj)
        {
            obj.StopEffect();
            Remove(obj);
        }

        public void KillInstance(EffectInstance obj)
        {
            obj.KillEffect();
            Remove(obj);
        }

        protected void AppendExecutor(float bonusDuration)
        {
            float durationBefore = _sequence.Duration();
            Debug.Log($"bonusDuration: {bonusDuration}");
            _sequence.IntervalAction(
                Trigger,
                bonusDuration,
                GlobalConfigs.EffectDelay
            );
            _sequence.AppendInterval(0.01f);
            
            float durationAfter = _sequence.Duration();
            Debug.Log($"Sequence Duration Before={durationBefore}, After={durationAfter}, BonusDuration={bonusDuration}");
        }

        public void Start()
        {
            if (!_sequence.IsPlaying())
                _sequence.Play();
            if (_sequence.IsComplete()) 
                _sequence.Restart();
        }

        public void Trigger()
        {
            string log = $"Number of effects triggered : {Collection.Count}"
                .Space().Add("Elapsed Time: " + _sequence.Elapsed());
            Debug.Log(log);
            foreach (EffectInstance instance in _instances)
            {
                instance.TriggerEffect();
            }
        }

        public void Stop()
        {
            _sequence.Complete();
        }

        public void Kill()
        {
            Stop();
        }
    }
}