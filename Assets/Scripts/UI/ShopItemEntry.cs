using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemEntry : MonoBehaviour
{
    public ShopItem item;
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;

    private void OnValidate()
    {
        if (item)
        {
            if (itemImage && itemNameText && itemPriceText)
            {
                itemImage.sprite = item.itemSprite;
                itemNameText.text = item.itemName;
                itemPriceText.text = item.itemPrice.ToString();
            }
            else
            {
                Debug.LogWarning("Shop item entry UI not assigned.");
            }
        }
    }

    public void SelectItem()
    {
        ShopManager.Instance.SelectedItem = item;
    }
}
