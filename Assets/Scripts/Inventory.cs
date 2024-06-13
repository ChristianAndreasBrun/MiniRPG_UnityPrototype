using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    PlayerControl control;
    public Transform parentSlots;
    public List<InventorySlot> inventory;
    int slotSelected;
    public Color[] slotColors; //0 = No seleccionado. 1 = Seleccionado

    public GameObject chestPanel, chestSlot;
    public GameObject dropObject;

    RaycastHit2D hit;
    public LayerMask mask;
    public Animator anim;

    [Header("FX Audio")]
    [SerializeField] private AudioSource hitFX;
    [SerializeField] private AudioSource dropItemFX;
    [SerializeField] private AudioSource pickupItemFX;
    [SerializeField] private AudioSource useItemFX;
    [SerializeField] private AudioSource restoreLifeFX;
    [SerializeField] private AudioSource openChestFX;


    void Start()
    {
        anim = GetComponent<Animator>();
        control = GetComponent<PlayerControl>();
        InitInventory();
        SelectSlot(0);
    }

    //Inicializar invetario
    void InitInventory()
    {
        //Crea inventario
        for (int i = 0; i < parentSlots.childCount; i++)
        {
            inventory.Add(new InventorySlot(parentSlots.GetChild(i).gameObject));
        }
    }

    void Update()
    {
        //Seleciona el slot con los numeros del teclado
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        
        //Seleciona el slot con la rueda del raton
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            int current = slotSelected;
            int next = current + 1;
            if (next >= inventory.Count) next = 0;
            SelectSlot(next);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            int current = slotSelected;
            int next = current - 1;
            if (next < 0) next = inventory.Count -1;
            SelectSlot(next);
        }

        //Iteractuar con items
        if (inventory[slotSelected].item != null)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(inventory[slotSelected].item.keyUse))
                UseItem();
        }
        CheckDistanceChest();

        if (Input.GetKeyDown(KeyCode.F)) PickUp();
        if (Input.GetKeyDown(KeyCode.Q)) Drop();
    }


    //Pinta el slot de inventario seleccionado
    void SelectSlot(int slotIndex)
    {
        if (slotIndex >= inventory.Count) return;

        slotSelected = slotIndex;
        //Agregamos un idice para pintar el slot
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].slot.GetComponent<Image>().color = (i == slotIndex) ? slotColors[1] : slotColors[0];
        }
    }

    //Funcion para regocer objetos con un rayo detector
    void PickUp()
    {
        hit = Physics2D.Raycast(transform.position, Direction.GetDirection(control.dir), 1f, mask);
        if (hit == true)
        {
            ItemInGame itemInfo = hit.collider.GetComponent<ItemInGame>();
            if (itemInfo.item.canPick)
            {
                pickupItemFX.Play();
                AddItem(itemInfo);
            }
            else if (itemInfo.item.isBag) OpenBag();
        }
    }

    //Añade item y destruye el que esta en el mundo
    public void AddItem(ItemInGame itemGame)
    {
        //Predicate: para buscar el item en inventario
        InventorySlot slot = inventory.Find(x => x.item?.id == itemGame.item.id);
        if (slot != null)
        {
            //ya tenia ese item, asi que solo sumo
            slot.amount += itemGame.amount;
            Destroy(itemGame.gameObject);
            ReprintInventory();
            return;
        }

        int emptySlot = HasEmptySlot();
        if (emptySlot == -1)
        {
            //no tenia el item, pero no tengo espacio en el invetario
            ItemManager saveItem = inventory[slotSelected].item;
            int saveAmount = inventory[slotSelected].amount;

            inventory[slotSelected].item = itemGame.item;
            inventory[slotSelected].amount = itemGame.amount;

            itemGame.InitItem(saveItem, saveAmount);
            ReprintInventory();
            return;
        }
        else
        {
            //no tenia el item, pero tengo espacio en el invetario
            inventory[emptySlot].item = itemGame.item;
            inventory[emptySlot].amount = itemGame.amount;
            Destroy(itemGame.gameObject);
            ReprintInventory();
            return;
        }
    }

    //Añade un item a inventario
    public void AddItem(ItemManager item, int amount)
    {
        //Predicate: para buscar el item en inventario
        InventorySlot slot = inventory.Find(x => x.item?.id == item.id);
        if (slot != null)
        {
            //ya tenia ese item, asi que solo sumo
            slot.amount += amount;
            ReprintInventory();
            return;
        }

        int emptySlot = HasEmptySlot();
        if (emptySlot == -1)
        {
            //no tenia el item, pero no tengo espacio en el invetario
        }
        else
        {
            //no tenia el item, pero tengo espacio en el invetario
            inventory[emptySlot].item = item;
            inventory[emptySlot].amount = amount;
            ReprintInventory();
            return;
        }
    }

    //Quita un item de inventario
    public void RemoveItem(int amount = 1)
    {
        inventory[slotSelected].amount -= amount;
        if (inventory[slotSelected].amount <= 0)
        {
            inventory[slotSelected].item = null;
        }
        ReprintInventory();
    }

    //Soltar objecto seleccionado
    public void Drop()
    {
        if (inventory[slotSelected].item != null)
        {
            Vector2 dropPosition = (Vector2)transform.position - Direction.GetDirection(control.dir);
            GameObject newDrop = Instantiate(dropObject, dropPosition, Quaternion.identity);
            dropItemFX.Play();
            newDrop.GetComponent<ItemInGame>().InitItem(inventory[slotSelected].item, inventory[slotSelected].amount);
            RemoveItem(inventory[slotSelected].amount);
        }

    }

    //Comprueba si el inventario esta lleno
    int HasEmptySlot()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == null) return i; //Retorna si el slot esta lleno
        }
        return -1; //Retorna si el slot esta vacio
    }

    //Reimprime el invetario entero para quitar o añadir items
    void ReprintInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item !=  null)
            {
                //Cambia al sprite del item
                inventory[i].slot.transform.GetChild(0).GetComponent<Image>().sprite = inventory[i].item.icon;
                inventory[i].slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventory[i].amount.ToString();
            }
            else
            {
                //Cambia al sprite del slot vacio
                inventory[i].slot.transform.GetChild(0).GetComponent<Image>().sprite = null;
                inventory[i].slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    //Funcion par usar el item seleccionado 
    void UseItem()
    {
        if (inventory[slotSelected].item != null && (inventory[slotSelected].item.function != "" || inventory[slotSelected].item.function == "OpenBag"))
        {
            Invoke(inventory[slotSelected].item.function, 0);
            useItemFX.Play();
        }
    }


    // ! Estas funciones son llamadas desde el scriptable object del item
    //Restaurar vida con un item
    void RestoreLife()
    {
        HealPotion item = inventory[slotSelected].item as HealPotion;
        print($"Me restuara {item.lifeRegeneration} puntos de vida.");
        PlayerValues.life += item.lifeRegeneration;
        control.ReprintLifeBar();
        restoreLifeFX.Play();
        RemoveItem();
    }

    ItemInGame currentChest;

    //Desblquear cofre
    void UnlockChest()
    {
        //print("Abreee");
        KeyItem item = inventory[slotSelected].item as KeyItem;
        hit = Physics2D.Raycast(transform.position, Direction.GetDirection(control.dir), 1f, mask);

        if (hit == true)
        {
            currentChest = hit.collider.GetComponent<ItemInGame>();
            if (CheckOpenChest(item, currentChest.item))
            {
                openChestFX.Play();
                PrintChest(currentChest.item);
            }
        }
    }

    void OpenBag()
    {
        hit = Physics2D.Raycast(transform.position, Direction.GetDirection(control.dir), 1f, mask);

        if (hit == true)
        {
            openChestFX.Play();
            currentChest = hit.collider.GetComponent<ItemInGame>();
            PrintChest(currentChest.item);
        }
    }

    //Mira el item de delante para comprobar el id
    bool CheckOpenChest(KeyItem key, ItemManager chest)
    {
        //print($"Key: {key} - Chest: {chest}");
        if (key != null)
        {
            for (int i = 0; i < key.idChest.Length; i++)
            {
                if (key.idChest[i] == chest.id) return true;
            }
        }
        return false;
    }

    ChestItem chest;

    //Mini menu del Cofre al abrirlo
    void PrintChest(ItemManager item)
    {
        for (int i = chestPanel.transform.childCount -1; i >= 0; i--)
        {
            Destroy(chestPanel.transform.GetChild(i).gameObject);
        }

        chest = item as ChestItem;
        for (int i = 0; i < chest.loot.Count; i++)
        {
            GameObject newSlot = Instantiate(chestSlot, chestPanel.transform);
            newSlot.AddComponent<ItemInChest>().Init(chest.loot[i], this);

        }
        chestPanel.SetActive(true);
    }

    public void GetItemFromChest(ItemInChest item)
    {
        InventorySlot slot = inventory.Find(x => x.item?.id == item.item.id);

        if (HasEmptySlot() > 0 || slot != null)
        {
            chest.loot.Remove(item.item);
            AddItem(item.item, 1);
            Destroy(item.gameObject);

            if (chest.loot.Count == 0)
            {
                Destroy(currentChest.gameObject);
                chestPanel.SetActive(false);
            }
        }
    }

    //El mini invetario del cofre se cierra al alejarse
    void CheckDistanceChest()
    {
        if (currentChest != null)
        {
            float dist = Vector2.Distance(transform.position, currentChest.transform.position);
            if (dist > 1.2f)
            {
                currentChest = null;
                chestPanel.SetActive(false);
            }
        }
    }


    //Ataque con espadas
    RaycastHit2D hitAttack;
    bool canAttack = true;
    void SowrdAttack()
    {
        anim.SetBool("Attacking", canAttack);

        if (canAttack)
        {
            Debug.Log("Check Attack");
            SwordItem item = inventory[slotSelected].item as SwordItem;
            hitAttack = Physics2D.Raycast(transform.position, Direction.GetDirection(control.dir), item.distanceAttack, item.attackMask);
            if (hitAttack)
            {
                //Quita vida al enemigo
                hitAttack.collider.GetComponent<EnemyManager>().GetDamage(item.damage);
                hitFX.Play();
            }
            canAttack = false;
            StartCoroutine(AttackDelay());
            Invoke("ActiveAttack", item.speedAttack);
        }
    }
    void ActiveAttack() { canAttack = true; }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.50f);
        anim.SetBool("Attacking", false);
        Debug.Log(canAttack);

    }
}


[System.Serializable]
public class InventorySlot
{
    public GameObject slot;
    public ItemManager item;
    public int amount;


    //Constructor de Slot
    public InventorySlot(GameObject slot)
    {
        this.slot = slot;
    }
}