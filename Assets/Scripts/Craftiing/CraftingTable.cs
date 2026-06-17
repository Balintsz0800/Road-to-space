using Unity.VisualScripting;
using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    public float interactRange = 2f;
    public GameObject CraftingUi;
    public GameObject craftingTable;

    private Transform Player;
    private bool isOpen = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player =  GameObject.FindGameObjectWithTag("Player").transform;
        craftingTable = GameObject.FindGameObjectWithTag("CraftingTable");
        CraftingUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(Player.position, transform.position);
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                craftingTable = hit.transform.gameObject;
                
                if (craftingTable != null)
                {
                    ToggleUi();
                }
            }
        }
    }

    private void ToggleUi()
    {
        if (isOpen)
        {
            CloseUi();
        }
        else
        {
            OpenUi();
        }
    }

    private void CloseUi()
    {
        isOpen = false;
        CraftingUi.SetActive(false);
    }

    private void OpenUi()
    {
        isOpen = true;
        CraftingUi.SetActive(true);
    }
}
