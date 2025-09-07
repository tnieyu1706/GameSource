using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] Image loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] Canvas loadingCanvas;
        [SerializeField] Camera loadingCamera;
        [SerializeField] EventSystem loadingEventSystem;
        [SerializeField] SceneGroup[] sceneGroups;

        float _targetProgress;
        bool _isLoading;

        public readonly SceneGroupManager SceneGroupManager = new SceneGroupManager();

        void Awake()
        {
            SceneGroupManager.OnSceneLoaded += s => Debug.Log("Loaded: " + s);
            SceneGroupManager.OnSceneUnloaded += s => Debug.Log("Unloaded: " + s);
            SceneGroupManager.OnSceneGroupLoaded += () => Debug.Log("Scene Group Loaded");
        }

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        void Update()
        {
            if (!_isLoading) return;

            float currentFillAmount = loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Task LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0;
            _targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene index");
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.ProgressAction += target => _targetProgress = Mathf.Max(_targetProgress, target);

            EnableLoadCanvas();
            await SceneGroupManager.LoadSceneAsync(sceneGroups[index], progress);

            EnableLoadCanvas(false);
        }

        void EnableLoadCanvas(bool enable = true)
        {
            _isLoading = true;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
            loadingEventSystem.gameObject.SetActive(enable);
        }
    }
}