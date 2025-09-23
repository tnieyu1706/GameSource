using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class InteractInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI buttonTypeText;
        [SerializeField] private TextMeshProUGUI contentText;

        public void SetInteractInfo(string buttonType, string content)
        {
            buttonTypeText.text = buttonType;
            contentText.text = content;
        }
    }
}