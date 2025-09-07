using UnityEngine.UIElements;

namespace _Project.Scripts.UI
{
    public class LogElement : VisualElement
    {
        public Label logText;

        public LogElement()
        {
            logText = this.CreateChild<Label>("logText");
        }
    }
}