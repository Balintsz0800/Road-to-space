using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    public float pickupDistance;
    public TMP_Text pickupText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
            {
                WorldItem item = hit.collider.GetComponent<WorldItem>();

                if (item != null)
                {
                    item.Pickup();
                }
            }
        }
    }
}
