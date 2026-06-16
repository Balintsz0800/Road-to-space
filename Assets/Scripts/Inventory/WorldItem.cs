using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public Item item;
    public int amount = 1;

    public void Pickup()
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryManager.AddItem(item, amount))
        {
            Destroy(gameObject);
        }
    }
}
