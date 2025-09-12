using UnityEngine;

namespace _Project.Scripts.Render
{
    public class RenderTopDownPerspectiveFlag : MonoBehaviour
    {
        private static readonly Vector3 eulerAnglesTopDown = new Vector3(65, 0, 0);
        void Start()
        {
            transform.eulerAngles = eulerAnglesTopDown;
        }
    }
}