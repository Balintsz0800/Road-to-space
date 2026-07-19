using System;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public int Damage;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UseTool();
        }
    }

    private void UseTool()
    {
        if (InventoryManager.instance.selectedSlot < 0 || InventoryManager.instance.selectedSlot >= InventoryManager.instance.slots.Length)
        {
            return;
        }
        
        InventorySlot slot = InventoryManager.instance.slots[InventoryManager.instance.selectedSlot];
        InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

        if (inventoryItem == null || inventoryItem.item.itemtype != Item.ItemType.Tool)
        {
            return;
        }
        
        if (inventoryItem.currentDurability <= 0)
        {
            BreakTool(inventoryItem);
        }
    }
    
    private void BreakTool(InventoryItem inventoryItem)
    {
        inventoryItem.transform.SetParent(null);
        Destroy(inventoryItem.gameObject);
        InventoryManager.instance.RefreshHandItem();
    }
}
