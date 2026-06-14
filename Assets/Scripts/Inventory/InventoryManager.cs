using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public int maxStackedItems;
    public GameObject inventoryItemPrefab;
    public GameObject handItemPrefab;
    public InventorySlot[] slots;
    private int selectedSlot = -1;
    public Transform handPos;
    public Transform dropPoint;
    GameObject currentHandItem;

    void Start()
    {
        ChangeSelectedSlot(0);
    }
    
    void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 10)
            {
                ChangeSelectedSlot(number -1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropSelectedItem();
        }
    }

    private void DropSelectedItem()
    {
        InventorySlot slot = slots[selectedSlot];
        
        InventoryItem invItem = slot.GetComponentInChildren<InventoryItem>();

        if (invItem == null)
        {
            return;
        }
        
        Instantiate(invItem.item.itemPrefab, dropPoint.position, Quaternion.identity);

        invItem.count--;

        if (invItem.count <= 0)
        {
            Destroy(invItem.gameObject);
        }
        else
        {
            invItem.RefreshCount();
        }
        Debug.Log(selectedSlot);
        Debug.Log(invItem);
        Debug.Log(invItem.item);
        Debug.Log(dropPoint);
        ShowHandItem();
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            slots[selectedSlot].Deselect();
        }
        slots[newValue].Select();
        selectedSlot = newValue;

        ShowHandItem();
    }

    private void ShowHandItem()
    {
        if (currentHandItem)
        {
            Destroy(currentHandItem);
        }
        
        InventoryItem inventoryItem = slots[selectedSlot].GetComponentInChildren<InventoryItem>();

        if (inventoryItem == null)
        {
            return;
        }

        if (inventoryItem.item.handItemPrefab)
        {
            currentHandItem = Instantiate(inventoryItem.item.handItemPrefab, handPos);
        }
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null && itemInSlot.item == item && item.stackable && itemInSlot.count < maxStackedItems)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }
        
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot.GetComponentInChildren<InventoryItem>() == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    private void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.IniTaliseItem(item);
    }

    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = slots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
                return item;
            }
        }
        return null;
    }
}
