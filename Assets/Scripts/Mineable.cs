using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mineable : MonoBehaviour
{
    public int health;
    public Tools toolsScript;
    public Item dropItem; 
    public int amount = 3;
    
    public void Mine()
    {
        InventorySlot slot = InventoryManager.instance.slots[InventoryManager.instance.selectedSlot];
        InventoryItem invItem = slot.GetComponentInChildren<InventoryItem>();

        if (invItem != null && invItem.item.itemtype == Item.ItemType.Tool)
        {
            health -= toolsScript.Damage;
        }

        if (health <= 0 && invItem != null && invItem.item.itemtype == Item.ItemType.Tool)
        {
            BreakStone();
        }
    }

    private void BreakStone()
    {
        InventoryManager.instance.AddItem(dropItem, amount);
        Destroy(gameObject);
    }
}
