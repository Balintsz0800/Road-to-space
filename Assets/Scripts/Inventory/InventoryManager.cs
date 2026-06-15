using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public int maxStackedItems;
    public GameObject inventoryItemPrefab;
    public InventorySlot[] slots;
    private int selectedSlot = -1;
    public Transform dropPoint;
    public Transform handPos;
    private GameObject currentHandItem;

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
        
        RefreshHandItem();
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

        RefreshHandItem();
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            slots[selectedSlot].Deselect();
        }
        slots[newValue].Select();
        selectedSlot = newValue;

        RefreshHandItem();
    }

    private void RefreshHandItem()
    {
        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
            currentHandItem = null;
        }
        
        InventorySlot slot = slots[selectedSlot];
        InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

        if (inventoryItem == null || inventoryItem.item == null || inventoryItem.item.itemPrefab == null)
        {
            return;
        }
        
        currentHandItem = Instantiate(inventoryItem.item.itemPrefab, handPos);
        currentHandItem.transform.localPosition = Vector3.zero;
        currentHandItem.transform.localRotation = Quaternion.identity;

        foreach (var rigidBody in currentHandItem.GetComponentsInChildren<Rigidbody>())
        {
            rigidBody.isKinematic = true;
        }

        foreach (var collider in currentHandItem.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
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
