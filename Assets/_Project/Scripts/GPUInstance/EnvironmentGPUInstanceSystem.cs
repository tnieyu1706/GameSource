using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.GPUInstance
{
    public class EnvironmentGPUInstanceSystem : MonoBehaviour
    {
        public List<GPUInstanceSpawnRunning> runningInstances;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (runningInstances == null)
                return;
        
            runningInstances.ForEach(instance => instance.Setup());
        }

        // Update is called once per frame
        void Update()
        {
            runningInstances.ForEach(instance => instance.Render());
        }
    }
}

