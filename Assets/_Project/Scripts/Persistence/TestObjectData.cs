using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.Persistence
{
    public class TestObjectData : MonoBehaviour, IBindingData<BaseGameObjectPersData>
    {
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
        [field: SerializeField] public BaseGameObjectPersData Data { get; set; }

        public void Bind(BaseGameObjectPersData data)
        {
            this.BindDefault(data);
            this.Data.SetForTransform(transform);
        }

        public void SaveEntity()
        {
            this.Data.SetByTransform(transform);
        }

        public void LoadEntity()
        {
            Bind(this.Data);
        }
    }
}