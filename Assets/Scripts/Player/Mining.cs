using UnityEngine;

public class Mining : MonoBehaviour
{
    public float Range = 5f;
    private Tools toolsScript;

    private void Start()
    {
        toolsScript = GetComponentInChildren<Tools>();
    }
    
    // Update is called once per frame
    void Update()
    {
        InventorySlot slot = InventoryManager.instance.slots[InventoryManager.instance.selectedSlot];
        InventoryItem invItem = slot.GetComponentInChildren<InventoryItem>();
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Range))
            {
                Mineable mineable = hit.collider.GetComponentInParent<Mineable>();
                Tree tree =  hit.collider.GetComponentInParent<Tree>();
                Log log = hit.collider.GetComponentInParent<Log>();
                HalfLog halfLog =  hit.collider.GetComponentInParent<HalfLog>();

                if (mineable != null && invItem.item.itemtype == Item.ItemType.Pickaxe)
                {
                    mineable.Mine();
                    invItem.currentDurability--;
                    Debug.Log( "Durability: " + invItem.currentDurability );
                }
                
                if (tree != null && invItem.item.itemtype == Item.ItemType.Axe)
                {
                    tree.Damage();
                    invItem.currentDurability--;
                    Debug.Log( "Durability: " + invItem.currentDurability );
                }
                
                if (log != null && invItem.item.itemtype == Item.ItemType.Axe)
                {
                    log.Damage();
                    invItem.currentDurability--;
                    Debug.Log( "Durability: " + invItem.currentDurability );
                }
                
                if (halfLog != null && invItem.item.itemtype == Item.ItemType.Axe)
                {
                    halfLog.Damage();
                    invItem.currentDurability--;
                    Debug.Log( "Durability: " + invItem.currentDurability );
                }
            }
        }
    }
}
