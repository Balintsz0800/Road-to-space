using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public Sprite icon;
    public string description;
    public Item result;
    public int resultAmount = 1;
    public Ingredient[] ingredients;
}