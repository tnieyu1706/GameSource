using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 100f;
    public float rotateSpeed = 3f;

    float yaw = 0f;   // Xoay trái/phải
    float pitch = 0f; // Xoay lên/xuống

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(new Vector3(moveX, 0f, moveZ));
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * scroll * zoomSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.E))
            transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q))
            transform.Translate(Vector3.back * zoomSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Giữ chuột phải để xoay
        {
            yaw += Input.GetAxis("Mouse X") * rotateSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotateSpeed;
            pitch = Mathf.Clamp(pitch, -89f, 89f); // Giới hạn xoay lên xuống

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}