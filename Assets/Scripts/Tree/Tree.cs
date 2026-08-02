using System;
using UnityEditor;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    public Tools toolsScript;
    
    [SerializeField] private GameObject LogPrefab;
    [SerializeField] private Transform LogSpawn;
    
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
            ChopTree();
        }
    }

    private void ChopTree()
    {
        Instantiate(LogPrefab, LogSpawn.position, LogSpawn.rotation);
        
        Destroy(gameObject);
    }
}
