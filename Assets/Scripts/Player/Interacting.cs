using UnityEngine;

public class Interacting : MonoBehaviour
{
    public GameObject Text;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private GameObject gameobject; 
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.transform.IsChildOf(gameobject.transform) || hit.collider.gameObject == gameobject)
            {
                Text.SetActive(true);
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                   DoorAnimation door =  gameobject.GetComponent<DoorAnimation>();

                   if (door != null)
                   {
                       door.ToggleDoor();
                   }
                }
            }
            else
            {
                Text.SetActive(false);
            }
        }
        else
        {
            Text.SetActive(false);
        }
    }
}
