using System.Runtime.CompilerServices;
using UnityEngine;

public class Storage : MonoBehaviour
{
    private GameObject StorageUI;
    private MouseMovement mouseMovement;
    private GameObject camera;
    public float distance = 3f;
    private bool opened;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        StorageUI = GameObject.FindGameObjectWithTag("StorageUI");
        StorageUI.SetActive(false);
        mouseMovement = camera.GetComponent<MouseMovement>();
    }
    
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (Input.GetKeyDown(KeyCode.E) && !opened)
                {
                    opened = !opened;
                    StorageUI.SetActive(opened);
                    Cursor.visible = opened;
                    Cursor.lockState = opened ? CursorLockMode.None : CursorLockMode.Locked;
                    mouseMovement.enabled = false;
                }
            }
        }

        if (opened && Input.GetKeyDown(KeyCode.Escape))
        {
            StorageUI.SetActive(false);
            opened = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            mouseMovement.enabled = true;
        }
    }
}
