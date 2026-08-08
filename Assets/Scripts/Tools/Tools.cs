using System;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public int Damage;
    public Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (InventoryManager.instance.selectedSlot < 0)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && !Mining.instance.isAttacking)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mining.instance.Range))
            {
                Mining.instance.savedHit = hit;
                Mining.instance.isAttacking = true;
                
                anim.SetTrigger("Attack");
            }
        }
    }
    
    public void Hit()
    {
        InventorySlot slot = InventoryManager.instance.slots[InventoryManager.instance.selectedSlot];
        InventoryItem invItem = slot.GetComponentInChildren<InventoryItem>();

        if (invItem == null)
        {
            return;
        }
        
        Mineable mineable = Mining.instance.savedHit.collider.GetComponentInParent<Mineable>();
        Tree tree =  Mining.instance.savedHit.collider.GetComponentInParent<Tree>();
        Log log = Mining.instance.savedHit.collider.GetComponentInParent<Log>();
        HalfLog halfLog =  Mining.instance.savedHit.collider.GetComponentInParent<HalfLog>();

        if (mineable != null && invItem.item.itemtype == Item.ItemType.Pickaxe)
        {
            mineable.Mine(Damage);
            invItem.currentDurability--;
        }
        else if (tree != null && invItem.item.itemtype == Item.ItemType.Axe)
        {
            tree.Damage(Damage);
            invItem.currentDurability--;
            Debug.Log(invItem.currentDurability);
        }
        else if (log != null && invItem.item.itemtype == Item.ItemType.Axe)
        {
            log.Damage(Damage);
            invItem.currentDurability--;
            Debug.Log(invItem.currentDurability);
        }
        else if (halfLog != null && invItem.item.itemtype == Item.ItemType.Axe)
        {
            halfLog.Damage(Damage);
            invItem.currentDurability--;
            Debug.Log(invItem.currentDurability);
        }
        
        if (invItem.currentDurability <= 0)
        {
            BreakTool(invItem);
        }
    }

    public void EndAttack()
    {
       Mining.instance.EndAttack();
    }
    
    private void BreakTool(InventoryItem invItem)
    {
        invItem.transform.SetParent(null);
        Destroy(invItem.gameObject);
        InventoryManager.instance.RefreshHandItem();
    }
}