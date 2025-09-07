using _Project.Scripts.Effect;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestEffectManager : SerializedMonoBehaviour
{
    [Required] public IEffectTarget EffectTarget;
    [Required] public IEffectBehavior EffectBehaviorTemplate;
    [Required] public IEffectBehavior EffectBehavior2;

    private EffectInstance _effectInstance;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _effectInstance = EffectBehavior2.PackageEffectInstance(EffectTarget);
            EffectSystem.Instance.RegistryEffect(EffectTarget, _effectInstance);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _effectInstance = EffectBehaviorTemplate.PackageEffectInstance(EffectTarget);
            EffectSystem.Instance.RegistryEffect(EffectTarget, _effectInstance);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // EffectSystem.Instance.Get(EffectTarget)?.KillLastInstance(_effectInstance);
            EffectSystem.Instance.UnRegistryEffect(EffectTarget, _effectInstance);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            EffectSystem.Instance.UnRegistryEffect(EffectTarget);
        }
        

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log($"Number Effect Behaviors in EffectSystem : {EffectSystem.Instance.Collection.Count}" );
        }
    }
}