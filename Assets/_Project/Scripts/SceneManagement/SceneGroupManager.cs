using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        private SceneGroup _activeSceneGroup;

        private const int ProgressDelaySeconds = 100;

        public async Task LoadSceneAsync(SceneGroup sceneGroup, IProgress<float> progress, bool reloadDupScenes = false)
        {
            _activeSceneGroup = sceneGroup;

            await UnLoadScenesAsync();

            //ensure
            List<string> loadedScenes = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = _activeSceneGroup.Scenes.Count;

            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            foreach (var scene in _activeSceneGroup.Scenes)
            {
                if (reloadDupScenes == false && loadedScenes.Contains(scene.Name)) continue;

                var operation = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
                
                await Task.Delay(TimeSpan.FromSeconds(2.5f));

                operationGroup.Operations.Add(operation);

                OnSceneLoaded?.Invoke(scene.Name);
            }
            
            Debug.Log("Loading scenes: " + operationGroup.Operations.Count);

            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(ProgressDelaySeconds);
            }

            Scene activeScene =
                SceneManager.GetSceneByName(_activeSceneGroup.FindSceneNameByType(SceneType.SceneActive));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }
            
            OnSceneGroupLoaded?.Invoke();
        }

        public async Task UnLoadScenesAsync()
        {
            var scenes = new List<string>();
            var activeSceneName = SceneManager.GetActiveScene().name;
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                
                var sceneName = scene.name;
                
                if (sceneName.Equals(activeSceneName) || sceneName.Equals("Bootstrapper")) continue;
                
                scenes.Add(sceneName);
            }
            
            var operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                operationGroup.Operations.Add(operation);
                
                OnSceneUnloaded?.Invoke(scene);
            }

            while (!operationGroup.IsDone)
            {
                await Task.Delay(100); // tight loop
            }
            
            // Optional: UnloadUnusedAssets - unload all unused asset from memory 
            await Resources.UnloadUnusedAssets();
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);

        public bool IsDone => Operations.Count == 0 || Operations.All(o => o.isDone);

        public AsyncOperationGroup(int capacity)
        {
            Operations = new List<AsyncOperation>(capacity);
        }
    }
}