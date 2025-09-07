using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.GPUInstance
{
    [CreateAssetMenu(fileName = "GPUInstanceSpawnRunning", menuName = "Scriptable Objects/GPUInstance/GPUInstanceSpawnRunning")]
    [Serializable]
    public class GPUInstanceSpawnRunning : ScriptableObject
    {
        public List<GPUInstanceObjectData> instances;
        public GPUInstanceSpawnConfig spawnConfig;

        public void Setup()
        {
            spawnConfig.GPUInstanceSetupSpawn(instances);
        }

        public void Render()
        {
            foreach (GPUInstanceObjectData instance in instances)
            {
                instance.RenderGPUInstance();
            }
        }
    }
}

