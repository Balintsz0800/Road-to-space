using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;
    public Recipe currentRecipe;
    public Image previewImage;
    public TMP_Text tileText;
    public TMP_Text descriptionText;
    public RequirementRow[] requirementRows;
    public Button craftButton;

    private void Awake()
    {
        instance = this;
        if (craftButton != null)
        {
            craftButton.onClick.AddListener(Craft);
        }
    }

    public void SelectRecipe(Recipe recipe)
    {
        currentRecipe = recipe;

        previewImage.sprite = recipe.icon;
        tileText.text = recipe.recipeName;
        descriptionText.text = recipe.description;

        RefreshRequirements();
    }

    private void RefreshRequirements()
    {
        if (currentRecipe == null)
        {
            return;
        }

        bool canCraft = true;

        for (int i = 0; i < requirementRows.Length; i++)
        {
            if (i < currentRecipe.ingredients.Length)
            {
                Ingredient req = currentRecipe.ingredients[i];
                
                int owned = CountItem(req.item);
                
                requirementRows[i].SetRequirement(req.item, owned, req.amount);

                if (owned < req.amount)
                {
                    canCraft = false;
                }
            }
            else
            {
                requirementRows[i].Hide();
            }
        }
        craftButton.interactable = canCraft;
    }

    private int CountItem(Item item)
    {
        int total = 0;

        foreach (InventorySlot slot in InventoryManager.instance.slots)
        {
            InventoryItem inv = slot.GetComponentInChildren<InventoryItem>();

            if (inv != null && inv.item == item)
            {
                total += inv.count;
            }
        }
        return total;
    }

    public void Craft()
    {
        if (currentRecipe == null)
        {
            return;
        }

        foreach (Ingredient req in currentRecipe.ingredients)
        {
            if (CountItem(req.item) < req.amount)
            {
                return;
            }
        }

        foreach (Ingredient req in currentRecipe.ingredients)
        {
            RemoveItem(req.item, req.amount);
        }
        
        InventoryManager.instance.AddItem(currentRecipe.result, currentRecipe.resultAmount);
        
        RefreshRequirements();
    }

    private void RemoveItem(Item item, int amount)
    {
        foreach (InventorySlot slot in InventoryManager.instance.slots)
        {
            InventoryItem inv = slot.GetComponentInChildren<InventoryItem>();

            if (inv == null || inv.item != item)
            {
                continue;
            }
            
            int remove = Mathf.Min(amount, inv.count);
            
            inv.count -= remove;
            amount  -= remove;

            if (inv.count <= 0)
            {
                Destroy(inv.gameObject);
            }
            else
            {
                inv.RefreshCount();
            }

            if (amount <= 0)
            {
                break;
            }
        }
        InventoryManager.instance.RefreshHandItem();
    }
}