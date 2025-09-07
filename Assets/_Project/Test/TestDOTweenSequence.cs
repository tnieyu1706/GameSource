using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestDOTweenSequence : MonoBehaviour
{
    public float number = 0f;

    public float endNumber = 10f;

    public float duration = 10f;

    public float delay = 1f;

    private Sequence seq;
    int counter = 0;
    private int times = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        // Action1();
        // Action2();
    }

    private void Initialize()
    {
        seq = DOTween.Sequence().Play();
        Debug.Log($"Duration : {seq.Duration()}, Elapsed : {seq.Elapsed()}");
    }

    public List<Action> actions = new List<Action>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            actions.Add(() =>
            {
                Debug.Log($"Action count: {actions.Count}");
                Debug.Log($" Time: {times} - Duration: {seq.Duration()}, Elapsed : {seq.Elapsed()}");
            });
            float remainingTime = seq.Duration() - seq.Elapsed();
            seq.Kill();
            seq = DOTween.Sequence().Play();
            Action1(seq, remainingTime + 4f);
            Debug.Log($"After append: Duration={seq.Duration()}, Elapsed={seq.Elapsed()}");
        }
    }

    private void Action1(Sequence sequence, float duration)
    {
        ++times;
        sequence.IntervalAction(
            ExecuteAllActions,
            duration,
            1f
        );
    }

    private void ExecuteAllActions()
    {
        foreach (var action in actions)
        {
            action.Invoke();
        }
    }

    private void Action2()
    {
        seq.IntervalAction(
            () => Debug.Log("Hello world! After" + (++counter)),
            4f,
            1f
        );
    }

    private float NumberSetAction(float adjustmentNumber) => number + adjustmentNumber;
}