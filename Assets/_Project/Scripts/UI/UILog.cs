using System.Collections;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.Storage;
using _Project.Scripts.Utils;
using AwesomeAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.UI
{
    public class UILog : BehaviorSingleton<UILog>, IUIMangerIntializer
    {
        [SerializeField] private StyleSheet styleSheet;
        [SerializeField] private int capacity = 4;
        [SerializeField] private float showSeconds = 0.5f;
        [SerializeField] private float delaySeconds = 2f;

        [SerializeField, MinMaxSlider(-100f, 100f)]
        private Vector2 verticalRand;

        private VisualElement _logUISpawn;

        private LogElement[] _logElements;

        private int lastIndex;

        void Update()
        {
            //test 
            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("Pressed J");
                WriteLog("hello xin chao");
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                _logUISpawn.style.opacity = 0;
            }
        }

        public void InitializeView(VisualElement root)
        {
            if (_logElements == null)
            {
                _logElements = new LogElement[capacity];
            }
            
            root.styleSheets.Add(styleSheet);
            _logUISpawn = root.CreateChild("logSpawner");
            
            Debug.Log("logElements.Length : " + _logElements.Length);

            for (int i = 0; i < _logElements.Length; i++)
            {
                LogElement logUI = root.CreateChild<LogElement>("logElement");
                logUI.style.visibility = Visibility.Visible;
                logUI.style.opacity = 0f;

                _logElements[i] = logUI;
            }
            
            _logUISpawn.SendToBack();
        }

        private LogElement GetLogElement()
        {
            for (int i = 0; i < _logElements.Length; i++)
            {
                if (_logElements[i].style.opacity.value <= 0)
                {
                    lastIndex = i;
                    return _logElements[i];
                }
            }

            return null;
        }

        public void WriteLog(string logText)
        {
            LogElement logElement = GetLogElement() ?? _logElements[(lastIndex + 1) % capacity];

            if (logElement != null)
            {
                Debug.Log("logElement is find" + "lastIndex: " + lastIndex);
            }

            ShowLogElement(logElement, logText);
        }

        void ShowLogElement(LogElement logElement, string logText)
        {
            logElement.logText.text = logText;
            logElement.style.opacity = 1f;
            SetLogElementPosition(logElement, _logUISpawn.worldBound.position);

            StartCoroutine(FadeLogElement(logElement));
        }

        private void SetLogElementPosition(LogElement logElement, Vector2 position)
        {
            logElement.style.left = position.x;
            logElement.style.top = position.y + Random.Range(verticalRand.x, verticalRand.y);
        }

        private IEnumerator FadeLogElement(LogElement logElement)
        {
            yield return SingletonFactory
                .GetInstance<WaitForSecondsServiceLocator>()
                .GetService(showSeconds);
            
            while (logElement.style.opacity.value > 0)
            {
                logElement.style.opacity =
                    (logElement.style.opacity.value - (Time.deltaTime / delaySeconds));
                if (logElement.style.opacity.value <= 0)
                {
                    logElement.style.opacity = 0f;
                }

                yield return null;
            }
        }
    }
}