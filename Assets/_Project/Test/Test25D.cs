using UnityEngine;

public class Test25D : MonoBehaviour
{
    private Vector3 cameraPos;
    private Vector3 cameraToward;

    // Update is called once per frame
    void Update()
    {
        cameraPos = Camera.main.transform.position;
        cameraToward = cameraPos - transform.position;
        cameraToward.y = 0;
        
        transform.LookAt(cameraToward);
    }
}
