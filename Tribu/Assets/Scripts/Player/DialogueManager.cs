using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    private Queue<string> sentences;
    public bool isDialogueActive { get; private set; }

    private PlayerController playerController;
    private PlayerAttack playerAttack;

    void Start()
    {
        dialoguePanel.SetActive(false);
        sentences = new Queue<string>();
        isDialogueActive = false;
        playerController = FindObjectOfType<PlayerController>(); 
        playerAttack = FindObjectOfType<PlayerAttack>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        sentences.Clear();

        if (nameText != null)
        {
            nameText.text = dialogue.name;
        }

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        isDialogueActive = true;
        if (playerController != null)
        {
            playerController.SetInputEnabled(false);
        }
        if (playerAttack != null)
        {
            playerAttack.SetInputEnabled(false);
        }

        DisplayNextSentence();
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        if (nameText != null)
        {
            nameText.text = "";
        }
        isDialogueActive = false;
        if (playerController != null)
        {
            playerController.SetInputEnabled(true);
        }
        if (playerAttack != null)
        {
            playerAttack.SetInputEnabled(true);
        }
    }


    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

}

[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea(3, 10)]
    public string[] sentences;
}
