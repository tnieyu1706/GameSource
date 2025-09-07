using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Effect
{
    public class EffectBehavior : SerializedMonoBehaviour, IEffectBehavior, IEffectTarget
    {
        [SerializeField, Required] private IEffectData effectData;
        [SerializeField, Required] private IEffectLogic _effectLogic;
    
        public IEffectData EffectData => effectData;
        public IEffectLogic EffectLogic => _effectLogic;
    }
}
