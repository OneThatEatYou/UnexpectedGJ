using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    private static ShopManager instance;
    public static ShopManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<ShopManager>();

                if (!instance)
                {
                    Debug.LogError("ShopManager not found.");
                }
            }

            return instance;
        }
    }
    public InventoryManager InventoryManager => GameManager.Instance.inventoryManager;

    private ShopItem selectedItem;
    public ShopItem SelectedItem
    {
        get
        {
            return selectedItem;
        }
        set
        {
            selectedItem = value;
            UpdateDescription(selectedItem);
            SwitchPreview(selectedItem);
        }
    }

    public int Nuts
    {
        get
        {
            return InventoryManager.nuts;
        }
        set
        {
            InventoryManager.nuts = value;
            UpdatePlayerNut();
        }
    }

    public TextMeshProUGUI nutText;
    public TextMeshProUGUI descriptionText;
    public Animator itemAnimator;

    private void Start()
    {
        UpdatePlayerNut();
        //update item availability
    }

    void UpdatePlayerNut()
    {
        nutText.text = Nuts.ToString();
    }

    void UpdateDescription(ShopItem item)
    {
        descriptionText.text = item.itemDescription;
    }

    void SwitchPreview(ShopItem item)
    {
        if (item)
        {
            AnimatorOverrideController aoc = new AnimatorOverrideController(itemAnimator.runtimeAnimatorController);
            aoc["PlayerPreview"] = item.animationClip;
            itemAnimator.runtimeAnimatorController = aoc;
        }
        else
        {
            Debug.Log($"Animation clip for {item.itemName} is not assigned.");
        }
    }

    public void BuyItem()
    {
        if (SelectedItem)
        {
            if (Nuts < SelectedItem.itemPrice)
            {
                Debug.Log("Not enough nuts");
            }
            else
            {
                Nuts -= SelectedItem.itemPrice;
                InventoryManager.itemHashset.Add(SelectedItem);
                Debug.Log($"Bought {SelectedItem.itemName} for {SelectedItem.itemPrice}.");
            }
        }
    }

    public void ExitShop()
    {
        GameManager.Instance.ChangeScene(0);
    }
}
