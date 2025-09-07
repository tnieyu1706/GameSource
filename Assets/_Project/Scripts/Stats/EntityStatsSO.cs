using System;
using _Project.Scripts.General.Patterns.UnityTechnique;
using UnityEngine;

namespace _Project.Scripts.Stats {
    public interface IEntityStatsData
    {
        int Health { get; set; }
        int Mana { get; set; }
        float AttackPower { get; set; }
        float Defense { get; set; }
        float Speed { get; set; }
        float Level { get; set; }

        string GetInfo();
    }

    [Serializable]
    public class EntityStats: IEntityStatsData
    {
        [SerializeField] private int health;
        [SerializeField] private int mana;
        [SerializeField] private float attackPower;
        [SerializeField] private float defense;
        [SerializeField] private float speed;
        [SerializeField] private float level;

        public int Health
        {
            get => health;
            set => health = value;
        }

        public int Mana
        {
            get => mana; 
            set => mana = value;
        }

        public float AttackPower
        {
            get => attackPower;
            set => attackPower = value;
        }

        public float Defense
        {
            get => defense;
            set => defense = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public float Level
        {
            get => level;
            set => level = value;
        }

        public string GetInfo()
        {
            return "EntityStats Info"
                .NewLine().Add($"Health: {Health}")
                .NewLine().Add($"Mana: {Mana}")
                .NewLine().Add($"AttackPower: {AttackPower}")
                .NewLine().Add($"Defense: {Defense}")
                .NewLine().Add($"Speed: {Speed}")
                .NewLine().Add($"Level: {Level}");
        }
    }

    [CreateAssetMenu(fileName = "EntityStats", menuName = "Scriptable Objects/EntityStats")]
    public class EntityStatsSO : CacheEntityScriptable<EntityStats>
    {
    }
}

