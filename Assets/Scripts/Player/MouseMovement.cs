using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensivity = 100f;
    public Transform playerBody;
    public Transform orientation;
    public Transform cameraTarget;
    
    public float cameraYOffset = 0f;
    
    public float xRot = 0f;
    public float yRot = 0f;
    private PlayerMovement playerMovement;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody != null)
        {
            playerMovement = playerBody.GetComponent<PlayerMovement>();
        }
    }
    
    void Update()
    {
        float mouseX =  Input.GetAxisRaw("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY =  Input.GetAxisRaw("Mouse Y") * mouseSensivity * Time.deltaTime;
        
        yRot += mouseX;
        
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(xRot, 0, 0f);
        playerBody.rotation = Quaternion.Euler(0, yRot, 0f);
        
        orientation.rotation = playerBody.rotation;
        
    }

    void LateUpdate()
    {
        if (playerMovement != null)
        {
            Vector3 targetPos = playerBody.position + Vector3.up * cameraYOffset;
            transform.position = targetPos;
        }

        if (cameraTarget != null)
        {
            transform.position = cameraTarget.position;
        }
    }
}