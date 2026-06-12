using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensivity = 100f;
    public Transform playerBody;
    public Transform orientation;
    
    public float cameraYOffset = 0.8f;
    
    public float xRot = 0f;
    public float yRot = 0f;
    private PlayerMovement playerMovement;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (playerBody != null)
        {
            playerMovement = playerBody.GetComponent<PlayerMovement>();
        }
    }
    
    void Update()
    {
        float mouseX =  Input.GetAxisRaw("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY =  Input.GetAxisRaw("Mouse Y") * mouseSensivity * Time.deltaTime;
        
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);
        
        yRot += mouseX;
        orientation.Rotate(Vector3.up * mouseX);
        
    }

    void LateUpdate()
    {
        if (playerMovement != null)
        {
            Vector3 targetPos = new Vector3(playerBody.position.x, playerBody.position.y + cameraYOffset, playerBody.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
        }
    }
}