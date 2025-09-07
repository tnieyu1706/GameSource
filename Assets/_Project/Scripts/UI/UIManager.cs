using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.InputSystem;
using _Project.Scripts.Storage;
using _Project.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(DragDropManager))]
    [RequireComponent(typeof(UILog))]
    public class UIManager : BehaviorSingleton<UIManager>, IInputUIHandler
    {
        #region Global
        [FoldoutGroup("Global")]
        [SerializeField] private List<UIDisplayModeData> displayDatas;
        [FoldoutGroup("Global")]
        [SerializeField, Required] private GameObject inventoryObject;
        
        public InputUIReader InputUI { get; set; }
        
        #endregion
        
        #region Support
        [FoldoutGroup("Support")]
        [SerializeField] protected UIDocument uiDocument;
        [FoldoutGroup("Support")]
        [SerializeField] protected StyleSheet globalStyleSheet;
        
        private VisualElement _root;
        [FoldoutGroup("Support")]
        public UnityEvent<VisualElement> OnInitializeView; 
        
        #endregion

        protected override void Awake()
        {
            base.Awake();
            uiDocument = GetComponent<UIDocument>();
            InputUI = InputUIReader.Instance;
        }

        void Start()
        {
            InitializeStorage();
        }

        void InitializeStorage()
        {
            if (displayDatas == null || displayDatas.Count == 0) return;

            foreach (var display in displayDatas)
            {
                if (display.displayMode == UIDisplayMode.Hide)
                {
                    display.displayObj.SetActive(false);
                }
                else if (display.displayMode == UIDisplayMode.Show)
                {
                    display.displayObj.SetActive(true);
                }
                else if (display.displayMode == UIDisplayMode.FirstShowAndHide)
                {
                    display.displayObj.SetActive(true);
                    this.Invoke(() => display.displayObj.SetActive(false), 0.01f);
                }
            }
        }
        
        void OnEnable()
        {
            if (uiDocument == null)
                return;

            StartCoroutine(InitializeViewCoroutine());

            InputUI.OpenInventory += OpenInventoryView;
        }

        void OnDisable()
        {
            InputUI.OpenInventory -= OpenInventoryView;
        }

        void OpenInventoryView()
        {
            inventoryObject.SetActive(!inventoryObject.activeSelf);
        }
        
        IEnumerator InitializeViewCoroutine()
        {
            InitializeView();
            yield return null;
            OnInitializeView?.Invoke(_root);
            yield return null;
        }
        
        void InitializeView()
        {
            _root = uiDocument.rootVisualElement;
            _root.Clear();
            _root.styleSheets.Add(globalStyleSheet);
        }
    }
}