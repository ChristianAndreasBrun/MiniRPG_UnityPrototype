using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    public Vector2 teleportPosition;
    public Image startPanelEffect;
    public Image endPanelEffect;
    public float effectSpeed;
    public float delay;
    [SerializeField] private AudioSource startTransitionFX;
    [SerializeField] private AudioSource endTransitionFX;


    private void Start()
    {
        startPanelEffect.gameObject.SetActive(false);
        endPanelEffect.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(TeleportDelay(collision.GetComponent<PlayerControl>()));
        }
    }

    private IEnumerator TeleportDelay(PlayerControl playerControl)
    {
        startPanelEffect.gameObject.SetActive(true);
        startTransitionFX.Play();
        startPanelEffect.fillAmount = 0f;

        while (startPanelEffect.fillAmount < 1f)
        {
            startPanelEffect.fillAmount += Time.deltaTime * effectSpeed;
            yield return null;
        }

        TeleportPLayer(playerControl);

        yield return new WaitForSeconds(delay);
        endPanelEffect.gameObject.SetActive(true);
        endTransitionFX.Play();
        startPanelEffect.gameObject.SetActive(false);

        while (endPanelEffect.fillAmount > 0f)
        {
            endPanelEffect.fillAmount -= Time.deltaTime * effectSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(delay);
        endPanelEffect.gameObject.SetActive(false);
        endPanelEffect.fillAmount = 1f;
    }

    private void TeleportPLayer(PlayerControl playerControl)
    {
        playerControl.currentTile = teleportPosition;
        playerControl.nextTile = teleportPosition;
        playerControl.transform.position = teleportPosition;
    }
}
