using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : ScriptableObject
{
    public string id, itemName, description;
    public Sprite icon;
    public bool canPick = true;

    public KeyCode keyUse;
    public string function;

    public bool isBag = false;
}
