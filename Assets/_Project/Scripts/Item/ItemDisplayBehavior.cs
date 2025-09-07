using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Item
{
    public class ItemDisplayBehavior : MonoBehaviour
    {
        [SerializeField] ItemTypeData itemTypeData;
        SpriteRenderer _spriteRenderer;
        
        public ItemTypeData ItemTypeData
        {
            get => itemTypeData;
            set
            {
                itemTypeData = value;
                _spriteRenderer.sprite = itemTypeData?.icon;
            }
        }

        void Awake()
        {
            _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        }
    }
}