using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInGame : MonoBehaviour
{
    public ItemManager item;
    public int amount;
    SpriteRenderer icon;


    private void Start()
    {
        ReprintItem();
    }

    public void InitItem(ItemManager item, int amount)
    {
        this.item = item;
        this.amount = amount;
        ReprintItem();
    }

    void ReprintItem()
    {
        ItemManager copyItem = Instantiate(item);
        item = copyItem;
        if (icon == null) icon = GetComponent<SpriteRenderer>();
        if (item != null) icon.sprite = item.icon;
    }
}
