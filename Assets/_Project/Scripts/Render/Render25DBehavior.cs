using UnityEngine;

namespace _Project.Scripts.Render
{
    public class Render25DBehavior : MonoBehaviour
    {
        
        // Update is called once per frame
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
