using UnityEngine;

public class HalfLog : MonoBehaviour
{
    [SerializeField] private int maxHealth = 25;
    [SerializeField] private int currentHealth;
    public Item dropItem;
    public int amount = 3;
    
    void Start()
    {
        currentHealth = maxHealth;    
    }

    public void Damage(int damage)
    {
        InventorySlot slot = InventoryManager.instance.slots[InventoryManager.instance.selectedSlot];
        InventoryItem invItem = slot.GetComponentInChildren<InventoryItem>();

        if (invItem != null && invItem.item.itemtype == Item.ItemType.Axe)
        {
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            Break();
        }
    }
    
    private void Break()
    {
        InventoryManager.instance.AddItem(dropItem, amount);
        Destroy(gameObject);
    }
}
