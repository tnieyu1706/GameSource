using UnityEngine;

namespace _Project.Scripts.Stats
{
    public abstract class EntityStatsBehavior : MonoBehaviour
    {
        [SerializeReference] public IEntityStatsable Statsable;
    }
}