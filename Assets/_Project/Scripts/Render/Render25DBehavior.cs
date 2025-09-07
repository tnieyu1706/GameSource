using UnityEngine;

namespace _Project.Scripts.Render
{
    public class Render25DBehavior : MonoBehaviour
    {
        private Vector3 _cameraPos;
    
        // Update is called once per frame
        void Update()
        {
            _cameraPos = Camera.main.transform.position;
            transform.LookAt(Render25DExecutor.LookAtTarget(_cameraPos, transform.position));
        }
    }
}
