using UnityEngine;

[CreateAssetMenu(menuName = "Items/CraftingRecipie")]
public class CraftingRecipies : ScriptableObject
{
    public string recipeName;
    public Ingredient[] ingredients;
    public Item result;
    public int resultAmount = 1;

    public class Ingredient
    {
        public Item item;
        public int amount;
    }
}
