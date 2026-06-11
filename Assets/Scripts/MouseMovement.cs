using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensivity = 100f;
    public Transform playerBody;
    
    public float xRot = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    
    }
    
    void LateUpdate()
    {
        float mouseX =  Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY =  Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
        
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        
        playerBody.Rotate(Vector3.up * mouseX);
        
    }
}