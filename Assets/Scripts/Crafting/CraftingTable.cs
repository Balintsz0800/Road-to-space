using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    public GameObject CraftingUI;
    public float distance = 3f;
    private bool opened;

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    opened = !opened;
                    CraftingUI.SetActive(opened);
                    Cursor.visible = opened;
                    Cursor.lockState = opened ? CursorLockMode.None : CursorLockMode.Locked;
                }
            }
        }

        if (opened && Input.GetKeyDown(KeyCode.Escape))
        {
            CraftingUI.SetActive(false);
            opened = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
