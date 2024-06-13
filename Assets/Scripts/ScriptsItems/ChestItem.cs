using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ChestItem", menuName = "Item/ChesttItem")]
public class ChestItem : ItemManager
{
    public List<ItemManager> loot;
}
