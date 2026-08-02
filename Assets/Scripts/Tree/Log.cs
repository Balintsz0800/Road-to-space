using UnityEngine;

public class Log : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;
    
    public Tools toolsScript;
    
    [SerializeField] private GameObject halfLog1;
    [SerializeField] private GameObject halfLog2;
    [SerializeField] private Transform halfLogSpawn1;
    [SerializeField] private Transform halfLogSpawn2;
    void Start()
    {
        currentHealth = maxHealth;    
    }
    
    public void Damage()
    {
        InventorySlot slot = InventoryManager.instance.slots[InventoryManager.instance.selectedSlot];
        InventoryItem invItem = slot.GetComponentInChildren<InventoryItem>();

        if (invItem != null && invItem.item.itemtype == Item.ItemType.Axe)
        {
            currentHealth -= toolsScript.Damage;
        }

        if (currentHealth <= 0)
        {
            ChopLog();
        }
    }

    private void ChopLog()
    {
        Instantiate(halfLog1, halfLogSpawn1.position, halfLogSpawn1.rotation);
        Instantiate(halfLog2, halfLogSpawn2.position, halfLogSpawn2.rotation);
        
        Destroy(gameObject);
    }
}
