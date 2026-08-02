using UnityEngine;

public class HalfLog : MonoBehaviour
{
    [SerializeField] private int maxHealth = 25;
    [SerializeField] private int currentHealth;
    public Tools toolsScript;
    public Item dropItem;
    public int amount = 3;
    
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
        if (currentHealth <= 0 && invItem != null && invItem.item.itemtype == Item.ItemType.Axe)
        {
            Break();
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    private void Break()
    {
        InventoryManager.instance.AddItem(dropItem, amount);
        Destroy(gameObject);
    }
}
