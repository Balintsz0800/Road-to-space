using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequirementRow : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemAmount;

    public void SetRequirement(Item item, int owned, int needed)
    {
        gameObject.SetActive(true);

        itemImage.sprite = item.image;
        itemName.text = item.name;
        itemAmount.text = owned + "/" + needed;

        if (owned >= needed)
        {
            itemAmount.color = Color.green;
        }
        else
        {
            itemAmount.color = Color.red;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
