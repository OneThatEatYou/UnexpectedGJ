﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
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
            UpdateDescription(SelectedItem);
            SwitchPreview(SelectedItem);
            UpdateBuyButton(SelectedItem);
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
    public Button buyButton; 
    public Button equipButton;
    public Animator itemAnimator;
    public AudioClip selectSFX;
    public AudioClip buySFX;
    public AudioClip exitSFX;

    private void Start()
    {
        UpdatePlayerNut();
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
            aoc["PlayerPreview"] = item.shopIdleClip;
            itemAnimator.runtimeAnimatorController = aoc;
        }
        else
        {
            Debug.Log($"Animation clip for {item.itemName} is not assigned.");
        }

        AudioManager.PlayAudioAtPosition(selectSFX, transform.position, AudioManager.sfxMixerGroup);
    }

    void UpdateBuyButton(ShopItem item)
    {
        if (InventoryManager.itemHashset.Contains(item))
        {
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);

            if (InventoryManager.equippedItem == item)
            {
                //uninteractable
                //equipped
                equipButton.interactable = false;
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equipped";
            }
            else
            {
                //interactable
                //equip
                equipButton.interactable = true;
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
        }
    }

    public void BuyItem()
    {
        if (SelectedItem && !InventoryManager.itemHashset.Contains(SelectedItem))
        {
            if (Nuts < SelectedItem.itemPrice)
            {
                Debug.Log("Not enough nuts");

                AudioManager.PlayAudioAtPosition(exitSFX, transform.position, AudioManager.sfxMixerGroup);
            }
            else
            {
                Debug.Log($"Bought {SelectedItem.itemName} for {SelectedItem.itemPrice}.");

                Nuts -= SelectedItem.itemPrice;
                InventoryManager.itemHashset.Add(SelectedItem);
                EquipItem();

                AudioManager.PlayAudioAtPosition(buySFX, transform.position, AudioManager.sfxMixerGroup);
            }
        }

        UpdateBuyButton(SelectedItem);
    }

    public void EquipItem()
    {
        InventoryManager.equippedItem = SelectedItem;
        InventoryManager.equippedItemObj = SelectedItem.itemControllerObj;
        AudioManager.PlayAudioAtPosition(selectSFX, transform.position, AudioManager.sfxMixerGroup);
        UpdateBuyButton(SelectedItem);
    }

    public void ExitShop()
    {
        AudioManager.PlayAudioAtPosition(exitSFX, transform.position, AudioManager.sfxMixerGroup);
        GameManager.Instance.ChangeScene(0);
    }
}
