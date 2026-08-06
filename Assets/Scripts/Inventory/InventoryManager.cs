using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public int maxStackedItems;
    public GameObject inventoryItemPrefab;
    public InventorySlot[] slots;
    public int selectedSlot = -1;
    public Transform dropPoint;
    public Transform handPos;
    public GameObject currentHandItem;
    private Item currentHandItemType;

    public static InventoryManager instance; 
    
    void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Awake()
    {
        instance = this;
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
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            int newSlot = selectedSlot;

            if (scroll < 0f)
            {
                newSlot++;
            }
            else if (scroll > 0f)
            {
                newSlot--;
            }

            if (newSlot >= slots.Length)
            {
                newSlot = 0;
            }
            else if (newSlot < 0)
            {
                newSlot = slots.Length - 1;
            }

            if (newSlot != selectedSlot)
            {
                ChangeSelectedSlot(newSlot);
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
            invItem.transform.SetParent(null);
            Destroy(invItem.gameObject);
        }
        else
        {
            invItem.RefreshCount();
        }
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

    public void RefreshHandItem()
    {
        InventorySlot slot = slots[selectedSlot];
        InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

        if (inventoryItem == null || inventoryItem.item == null || inventoryItem.item.handPrefab == null)
        {
            if (currentHandItem != null)
            {
                Destroy(currentHandItem);
                currentHandItem = null;
                currentHandItemType =  null;
            }
            
            return;
        }

        if (currentHandItem != null && currentHandItemType == inventoryItem.item)
        {
            return;
        }

        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
        }
        
        currentHandItem = Instantiate(inventoryItem.item.handPrefab, handPos);
        
        currentHandItem.transform.localPosition = inventoryItem.item.handPosition;
        currentHandItem.transform.localRotation = Quaternion.Euler(inventoryItem.item.handRotation);
        currentHandItem.transform.localScale = inventoryItem.item.handScale;

        currentHandItemType = inventoryItem.item;

        foreach (Rigidbody rb in currentHandItem.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }

        foreach (Collider collider in currentHandItem.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    public bool AddItem(Item item, int amount)
    {
        if (item.stackable)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                InventoryItem existing = slots[i].GetComponentInChildren<InventoryItem>();

                if (existing != null && existing.item == item && existing.count < maxStackedItems)
                {
                    existing.count += amount;
                    existing.RefreshCount();
                    if (selectedSlot == i)
                    {
                        RefreshHandItem();
                    }
                    
                    return true;
                }
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].GetComponentInChildren<InventoryItem>() == null)
                {
                    SpawnNewItem(item, slots[i], amount);
                    if (selectedSlot == i)
                    {
                        RefreshHandItem();
                    }
                    return true;
                }
            }
            return false;
        }

        for (int x = 0; x < amount; x++)
        {
            bool placed = false;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].GetComponentInChildren<InventoryItem>() == null)
                {
                    SpawnNewItem(item, slots[i], 1);
                    if (selectedSlot == i)
                    {
                        RefreshHandItem();
                    }
                    placed = true;
                    break;
                }
            }
            if (!placed)
            {
                return false;
            }
        }
        return true;
    }

    private void SpawnNewItem(Item item, InventorySlot slot, int amount)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.IniTaliseItem(item);
        
        inventoryItem.count = amount;
        inventoryItem.RefreshCount();
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
                    itemInSlot.transform.SetParent(null);
                    Destroy(itemInSlot.gameObject);
                    RefreshHandItem();
                }
                else
                {
                    itemInSlot.RefreshCount();
                    RefreshHandItem();
                }
                return item;
            }
        }
        return null;
    }
}