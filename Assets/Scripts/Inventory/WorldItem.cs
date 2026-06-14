using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public Item item;

    public void Pickup()
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryManager.AddItem(item))
        {
            Destroy(gameObject);
        }
    }
}
