using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.GPUInstance
{
    [CreateAssetMenu(fileName = "GPUInstanceSpawnConfig", menuName = "Scriptable Objects/GPUInstance/GPUInstanceSpawnConfig")]
    [Serializable]
    public class GPUInstanceSpawnConfig : ScriptableObject
    {
        [TabGroup("Base Requirement")] public Texture2D maskTexture;
        [TabGroup("Base Requirement")] public float threshold = 0.36f;
    
        [TabGroup("Base Requirement")] [Required]
        public Transform objectPlane;
    
        [TabGroup("Base Requirement")] public float maskScaleValue = 20f;
        [TabGroup("Base Requirement")] public string layerRaycastName = "Terrain";
    
        [TabGroup("Options")] public float rayWeight = 1000;
        [TabGroup("Options")] public float rayHeight = 20;
        [TabGroup("Options")] public float raycastDistance = 40;
        [TabGroup("Options")] public float scaleWeightDivValue = 10f;
    
        public void GPUInstanceSetupSpawn(List<GPUInstanceObjectData> instances)
        {
            if (objectPlane == null)
                return;
    
            float scaleWeight = objectPlane.localScale.x / scaleWeightDivValue;
            float rayHeightUse = rayHeight * rayWeight;
            float raycastDistanceUse = raycastDistance * rayWeight;
    
            List<Vector2> maskPixels = MaskExecutor.GetSatisfiedMaskPixels(maskTexture, threshold);
    
            foreach (GPUInstanceObjectData instance in instances)
            {
                if (instance.isInitialized)
                    continue;
    
                int renderCountNumber = 0;
    
                instance.matrices = new Matrix4x4[instance.renderCount];
    
                for (int i = 0; i < instance.renderCount; i++)
                {
                    int pixelRand = Random.Range(0, maskPixels.Count);
                    Vector2 worldPixel =
                        maskPixels[pixelRand]
                            .ConvertPixelToWorld(maskTexture.width - 1, maskTexture.height - 1)
                            .ApplyScale(objectPlane.localScale.ConvertToVector2() * maskScaleValue)
                            .ApplyPosition(objectPlane.position.ConvertToVector2());
    
                    Vector3 startPos = new Vector3(worldPixel.x, rayHeightUse, worldPixel.y);
                    Ray ray = new Ray(startPos, Vector3.down);
    
                    if (Physics.Raycast(ray, out RaycastHit hit, raycastDistanceUse, LayerMask.GetMask(layerRaycastName)))
                    {
                        Vector3 pos = hit.point;
                        Vector3 scale = Vector3.one *
                                        (instance.scaleInstanceOptions.GetRandomNumber() * instance.scaleWeight) *
                                        scaleWeight;
                        Quaternion rot = Quaternion.Euler(instance.rotateOffset.Add(0, Random.Range(0, 360), 0));
    
                        instance.matrices[i] = Matrix4x4.TRS(pos, rot, scale);
                        renderCountNumber++;
                    }
                }
    
                Debug.Log(
                    $"GPU Instance initialize: {instance.name} - renderCount: {renderCountNumber}/{instance.renderCount}");
    
                instance.isInitialized = true;
            }
        }
    }
}

