using _Project.Scripts.Item;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Effect
{
    [CreateAssetMenu(fileName = "EffectType", menuName = "Scriptable Objects/EffectSystem/EffectType")]
    public class EffectType : ScriptableObject, IBaseTypeData
    {
        [SerializeField, ReadOnly] private int effectTypeId;
        [SerializeField] private string effectTypeName;

        void Awake()
        {
            effectTypeId = IdentifySystemHelper.GenerateId(this);
        }

        public int TypeId
        {
            get => effectTypeId;
            set => effectTypeId = value;
        }

        public string Name => effectTypeName;
    }
}