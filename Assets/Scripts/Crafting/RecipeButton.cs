using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    public Recipe recipe;
    
    public Image iconImage;
    public TMP_Text nameText;

    private void Start()
    {
        if (recipe == null)
        {
            return;
        }
        iconImage.sprite = recipe.icon;
        nameText.text = recipe.recipeName;
    }

    public void Select()
    {
        CraftingManager.instance.SelectRecipe(recipe);
    }
}
