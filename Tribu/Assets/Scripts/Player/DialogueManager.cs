using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMesh Pro

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    private bool isDialogueActive;

    void Start()
    {
        dialoguePanel.SetActive(false);
        sentences = new Queue<string>();
        isDialogueActive = false;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        isDialogueActive = true;
        DisplayNextSentence();
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

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        isDialogueActive = false;
        Debug.Log("Diálogo finalizado");
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextSentence();
        }
    }
}
[System.Serializable]
public class Dialogue
    {
        public string name;
        [TextArea(3, 10)]
        public string[] sentences;
    }