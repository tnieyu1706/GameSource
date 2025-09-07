using System;
using _Project.Scripts.Interact;
using Sirenix.OdinInspector;
using _Project.Scripts.Stats;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerStatsInteractable : MonoBehaviour, IEntityStatsInteractable
    {
        [SerializeField] private EntityStatsSO statsSO;
        public IEntityStatsData EntityStats => statsSO.entity;
        public void Accept(IEntityStatsInteractor interactor)
        {
            interactor.HandleEntityStats(EntityStats);
        }

    }
}

