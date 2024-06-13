using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SwordItem", menuName = "Item/SwordItem")]
public class SwordItem : ItemManager
{
    public float distanceAttack = 1;
    public LayerMask attackMask;
    public float speedAttack;
    public int damage;
    //public int durability;
}
