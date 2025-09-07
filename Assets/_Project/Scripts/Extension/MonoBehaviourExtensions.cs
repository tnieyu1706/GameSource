using System;
using System.Collections;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.Utils;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void Invoke(this MonoBehaviour monoBehaviour, Action action, float delay)
    {
        monoBehaviour.StartCoroutine(InvokeCoroutine(action, delay));
    }
    
    public static IEnumerator InvokeCoroutine(Action action, float delay)
    {
        yield return SingletonFactory.GetInstance<WaitForSecondsServiceLocator>().GetService(delay);
        
        action?.Invoke();
    }
}