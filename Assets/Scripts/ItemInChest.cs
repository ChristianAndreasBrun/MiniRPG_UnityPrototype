using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInChest : MonoBehaviour
{
    public ItemManager item;
    Inventory inventory;

    public void Init(ItemManager item, Inventory inventory)
    {
        this.item = item;
        this.inventory = inventory;
        transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
        GetComponent<Button>().onClick.AddListener(delegate { PressItem(); });
    }

    public void PressItem()
    {
        inventory.GetItemFromChest(this);
    }
}
