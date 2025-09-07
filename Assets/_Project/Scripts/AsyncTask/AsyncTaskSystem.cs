using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using _Project.Scripts.General.Patterns.Singleton;
using UnityEngine;

namespace _Project.Scripts.AsyncTask
{
    public class AsyncTaskSystem : BehaviorSingleton<AsyncTaskSystem>
    {
        private readonly ConcurrentQueue<IEnumerator> mainThreadActions = new();

        void Update()
        {
            while (mainThreadActions.TryDequeue(out var action))
            {
                try
                {
                    StartCoroutine(action);
                }
                catch (Exception e)
                {
                    Debug.LogError("AsyncTaskSystem action exception: " + e);
                }
            }
        }

        /// <summary>
        /// Enqueue action for mainThread.
        /// </summary>
        /// <param name="action"></param>
        public void RunOnMainThread(IEnumerator action)
        {
            mainThreadActions.Enqueue(action);
        }

        /// <summary>
        /// Run AsyncBackgroundTask, after Completed => Enqueue mainAction for mainThread to run
        /// AsyncBackgroundTask: Not Unity API
        /// mainAction: Use UnityAPI
        /// </summary>
        /// <param name="asyncBackgroundTask"></param>
        /// <param name="mainAction"></param>
        public void RunAsync(Task asyncBackgroundTask, IEnumerator mainAction)
        {
            Task.Run(async () => await asyncBackgroundTask)
                .ContinueWith(t =>
                {
                    if (t.IsCompleted)
                        mainThreadActions.Enqueue(mainAction);
                });
        }
    }
}