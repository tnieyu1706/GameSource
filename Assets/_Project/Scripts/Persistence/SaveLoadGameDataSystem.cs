using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.General.Patterns.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Persistence
{
    public class SaveLoadGameDataSystem : BehaviorSingleton<SaveLoadGameDataSystem>
    {
        private static string _fileSaveNameStatic = "GameData";
        public GameData gameData;
        public string fileSaveName = _fileSaveNameStatic;
        private List<IBindingData> entityBindings = new();

        private GameDataService _gameDataService;

        protected override void Awake()
        {
            base.Awake();
            _gameDataService = new GameDataService(new JsonSerializer());
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoadedAction;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoadedAction;
        }

        void OnSceneLoadedAction(Scene scene, LoadSceneMode mode)
        {
            DefaultBindings();
        }

        void DefaultBindings()
        {
            // Bind<TestObjectData, BaseGameObjectPersData>(gameData.baseGameObjectPersData);
        }

        void Bind<TRuntime, TData>(TData data)
            where TRuntime : MonoBehaviour, IBindingData<TData>
            where TData : IPersistenceSavable, new()
        {
            var entity = Object.FindFirstObjectByType<TRuntime>();
            
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData()
                    {
                        Id = entity.Id
                    };
                }

                entity.Bind(data);
                entityBindings.Add(entity);
            }
        }
        
        void Bind<TRuntime, TData>(List<TData> datas)
            where TRuntime : MonoBehaviour, IBindingData<TData>
            where TData : IPersistenceSavable, new()
        {
            var entities = Object.FindObjectsByType<TRuntime>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(x => x.Id == entity.Id);
                if (data == null)
                {
                    data = new TData() {Id = entity.Id};
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        [Button]
        public void NewGame()
        {
            gameData = new GameData()
            {
                Name = "New Game",
                GameLevel = "Demo"
            };
            
        }

        [Button]
        public void Save()
        {
            if (gameData == null)
            {
                Debug.LogError("GameData is null");
                return;
            }

            foreach (var entity in entityBindings)
            {
                entity.SaveEntity();
            }
            
            _gameDataService.Save(fileSaveName, gameData);
        }

        [Button]
        public void Load()
        {
            gameData = _gameDataService.Load(fileSaveName);
            entityBindings.Clear();
            DefaultBindings();
        }

        [Button]
        public void Delete()
        {
            gameData = null;
            _gameDataService.Delete(fileSaveName);
        }

        [Button]
        public void ResetFileSaveNameStatic()
        {
            _fileSaveNameStatic = fileSaveName.ToString();
        }
    }
}