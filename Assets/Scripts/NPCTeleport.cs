using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTeleport : MonoBehaviour
{
    //public Transform teleportPoint;
    public GameObject npcToTeleport;
    public GameObject npcToDestroy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TeleportNPC();
            Destroy(npcToDestroy);
        }
    }

    private void Start()
    {
        npcToTeleport.SetActive(false);

    }

    private void TeleportNPC()
    {
        npcToTeleport.SetActive(true);
    }
}
