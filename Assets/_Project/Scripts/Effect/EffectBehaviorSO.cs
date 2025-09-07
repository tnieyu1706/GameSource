using _Project.Scripts.Item;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Effect
{
    [CreateAssetMenu(fileName = "EffectBehavior", menuName = "Scriptable Objects/EffectSystem/EffectBehavior")]
    public class EffectBehaviorSo : ScriptableObject, IEffectBehavior, IBaseTypeData
    {
        [SerializeField] private int effectId;
        [SerializeField] private string effectName;
        [SerializeReference] private IEffectData effectData;
        [SerializeReference] private IEffectLogic effectLogic;

        void Awake()
        {
            effectId = IdentifySystemHelper.GenerateId(this);
        }

        public int TypeId
        {
            get => effectId;
            set => effectId = value;
        }
        public string Name => effectName;
        public IEffectData EffectData => effectData;
        public IEffectLogic EffectLogic => effectLogic;
    }
}