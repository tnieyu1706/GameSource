using System;
using _Project.Scripts.InputSystem;
using UnityEngine;

public class InputGameplayTest : MonoBehaviour, IInputGameplayHandler
{
    public InputGameplayReader InputGamePlay { get; set; }

    void Awake()
    {
        InputGamePlay = InputGameplayReader.Instance;
    }
    
    void Start()
    {
        InputGamePlay.Interact += TestFunc;
    }

    void TestFunc()
    {
        Debug.Log("Press E - Interact");
        
        InputGamePlay.Interact -= TestFunc;
    }
}