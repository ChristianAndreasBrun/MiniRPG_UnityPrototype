using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class DialogueCharacter
{
    public string name;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3,10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialoguesManager : MonoBehaviour
{
    public MenuManger manger;
    public Image panelBox;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    [SerializeField] private AudioSource talkFX;

    private Queue<DialogueLine> queueLines;
    private bool isTyping = false;
    public bool endGame = false;

    public Dialogue dialogue;
    public float typingSpeed;

    private void Start()
    {
        panelBox.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopDialogue();
            if (endGame)
            {
                manger.GameOver();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && isTyping) 
        {
            if (isTyping) 
            {
                CompleteTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue()
    {
        panelBox.gameObject.SetActive(true);
        queueLines = new Queue<DialogueLine>(dialogue.dialogueLines);
        StartCoroutine(TypeDialogue());
    }
     
    private void NextLine()
    {
        if (queueLines.Count > 0)
        {
            isTyping = false;
            dialogueArea.text = queueLines.Peek().line;
        }
    }

    private void StopDialogue()
    {
        talkFX.Stop();
        StopAllCoroutines();
        panelBox.gameObject.SetActive(false);
    }

    IEnumerator TypeDialogue()
    {
        while (queueLines.Count > 0)
        {
            DialogueLine currentLine = queueLines.Dequeue();
            characterName.text = currentLine.character.name;
            dialogueArea.text = "";

            foreach (char letter in currentLine.line.ToCharArray())
            {
                dialogueArea.text += letter;
                yield return new WaitForSeconds(typingSpeed);
                talkFX.Play();
            }
            
            isTyping = true;
            while (isTyping)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
        panelBox.gameObject.SetActive(false);
    }

    private void CompleteTyping()
    {
        isTyping = false;
    }


}
