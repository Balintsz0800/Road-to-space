using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    public float interactRange = 2f;
    public GameObject CraftingUi;

    private Transform Player;
    private bool isOpen = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player =  GameObject.FindGameObjectWithTag("Player").transform;
        CraftingUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float distance =  Vector3.Distance(Player.position, transform.position);

        if (distance <= interactRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleUi();
            }
            else if (isOpen)
            {
                CloseUi();
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
