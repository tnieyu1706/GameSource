using System;
using System.Collections.Generic;
using _Project.Scripts.Config;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.GPUInstance
{
    [CreateAssetMenu(fileName = "GPUInstanceData", menuName = "Scriptable Objects/GPUInstance/GPUInstanceData")]
    [Serializable]
    public class GPUInstanceObjectData : ScriptableObject
    {
        public bool isInitialized = false;
        public int renderCount = 100;
        public Mesh mesh;
        public List<Material> materials;
        [HideInInspector] public Matrix4x4[] matrices;
        public float scaleWeight = 1f;
        public Vector2 scaleInstanceOptions = new Vector2(0.8f, 1.5f);
        public Vector3 rotateOffset = Vector3.zero;
    
        private int GetInstanceMinDraw()
        {
            if (mesh ==null || materials == null || materials.Count <= 0)
            {
                return 0;
            }
            return Mathf.Min(mesh.subMeshCount, materials.Count);
        }
    
        public void RenderGPUInstance()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("GPU Instance Data is not initialized.");
                return;
            }
            if (matrices == null)
            {
                Debug.LogError($"GPU Instance Data {name} is missing matrices!");
                return;
            } 
            for (int i = 0; i < renderCount; i+=GlobalConfigs.DrawLimitSize)
            {
                int instanceRender = Mathf.Min(GlobalConfigs.DrawLimitSize, renderCount - i);
                for (int m = 0; m < GetInstanceMinDraw(); m++)
                {
                    Graphics.DrawMeshInstanced(mesh, m, materials[m], matrices, instanceRender );
                }
            }
            
        }
    }
}

