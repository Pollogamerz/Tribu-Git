using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalParty : MonoBehaviour, IStartable, IMinigameIndex
{
    [Header("Animals components")]
    public List<AnimalDataSO> animals;
    public List<Button> animalButtons;
    public List<SpriteRenderer> animalSprites;
    public Button finishDanceButton;

    [Header("Dialogue components")]
    public DialogueManager _dialogueManager;
    public string[] introductionLines;
    public Button nextDialogueLineButton;
    public Image keyboardInputSprite;

    public event Action OnMiniGameFinished;

    private bool _waitingForInput = false;

    void Start()
    {
        finishDanceButton.onClick.AddListener(FinishDance);
        SetButtonsInteractable(false);
    }

    void Update()
    {
        if (_waitingForInput && Input.GetKeyDown(KeyCode.E))
        {
            _waitingForInput = false;
        }
    }

    public void OnNextDialogeLineInput()
    {
        _waitingForInput = false;
    }

    private IEnumerator ShowIntroductionDialogue()
    {
        nextDialogueLineButton.interactable = true;
        keyboardInputSprite.gameObject.SetActive(true);
        SetButtonsInteractable(false);

        for (int i = 0; i < introductionLines.Length; i++)
        {
            Dialogue dialogue = new Dialogue
            {
                name = "Narrador",
                sentences = new string[] { introductionLines[i] }
            };

            _dialogueManager.StartDialogue(dialogue, null);
            _waitingForInput = true;

            yield return new WaitUntil(() => !_waitingForInput);
        }

        nextDialogueLineButton.interactable = false;
        keyboardInputSprite.gameObject.SetActive(false);
        _dialogueManager.EndDialogue();

        SetButtonsInteractable(true);
    }

    public void StartMiniGame()
    {
        SetupSceneSprites();
        SetupButtons();
        StartCoroutine(ShowIntroductionDialogue());
    }

    private void SetupSceneSprites()
    {
        for (int i = 0; i < animals.Count && i < animalSprites.Count; i++)
        {
            var spriteRenderer = animalSprites[i];
            var animal = animals[i];

            spriteRenderer.sprite = animal.animalSprite;

            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            float targetHeight = 1.5f;
            float scaleFactor = targetHeight / spriteSize.y;

            spriteRenderer.transform.localScale = new Vector3(2f * scaleFactor, 2f * scaleFactor, 1f);

            Animator animator = spriteRenderer.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = animal.animalIdleAnimator;
            }
        }
    }

    private void SetupButtons()
    {
        for (int i = 0; i < animals.Count && i < animalButtons.Count; i++)
        {
            AnimalDataSO animal = animals[i];
            Button button = animalButtons[i];

            Image btnImage = button.GetComponent<Image>();
            if (btnImage != null)
                btnImage.sprite = animal.animalSprite;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnAnimalButtonClicked(animal));
        }
    }

    private void OnAnimalButtonClicked(AnimalDataSO animal)
    {
        int index = animals.IndexOf(animal);

        Animator animator = animalSprites[index].GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Dance");
        }
    }

    private void FinishDance()
    {
        FinishMiniGame();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in animalButtons)
        {
            btn.gameObject.SetActive(interactable);
            btn.interactable = interactable;
        }
    }

    public void FinishMiniGame()
    {
        SetButtonsInteractable(false);
        OnMiniGameFinished?.Invoke();
    }

    public void SetupMinigame(int minigameIndex) { }
}
