using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensivity = 100f;
    public Transform playerBody;
    
    public float xRot = 0f;
    public float yRot = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    
    }
    
    void Update()
    {
        float mouseX =  Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY =  Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
        
        xRot -= mouseY;
        xRot = Mathf.Clamp(mouseX, -90f, 90f);
        yRot += mouseX;
        
        transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);
        
        playerBody.Rotate(Vector3.up * mouseX);
        
    }
}