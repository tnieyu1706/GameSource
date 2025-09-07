using System;
using UnityEngine.Serialization;

namespace _Project.Scripts.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public Action<float> ProgressAction { get; set; }

        private const float Ratio = 1f;

        public void Report(float value)
        {
            ProgressAction?.Invoke(value / Ratio);
        }
    }
}