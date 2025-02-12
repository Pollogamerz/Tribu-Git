using UnityEngine;
using UnityEngine.Events;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactionUI;
    public DialogueManager dialogueManager;
    public Dialogue dialogue;
    private bool isPlayerInRange;
    public Animator animator;

    [Header("Configuración del NPC")]
    public bool activatesEvent;
    public UnityEvent onDialogueEnd;

    void Start()
    {
        interactionUI.SetActive(false);
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (animator != null)
        {
            animator.SetBool("isIdle", true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactionUI.SetActive(true);
            if (!dialogueManager.isDialogueActive && dialogueManager.nameText != null)
            {
                dialogueManager.nameText.text = dialogue.name;
            }
        }

        if (animator != null)
        {
            animator.SetBool("isTalking", true);
            animator.SetBool("isIdle", false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionUI.SetActive(false);
            dialogueManager.EndDialogue();
        }

        if (animator != null)
        {
            animator.SetBool("isTalking", false);
            animator.SetBool("isIdle", true);
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueManager.isDialogueActive)
            {
                dialogueManager.DisplayNextSentence();
            }
            else
            {
                interactionUI.SetActive(false);
                dialogueManager.StartDialogue(dialogue, this);
            }
        }
    }


    public void OnDialogueEnd()
    {
        if (activatesEvent)
        {
            onDialogueEnd.Invoke();
        }
        if (activatesEvent)
        {
            Debug.Log("Evento activado por: " + gameObject.name);
            onDialogueEnd.Invoke();
        }
    }
}
