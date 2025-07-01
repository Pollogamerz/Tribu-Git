using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalDescriptionQuiz : MonoBehaviour, IStartable, IMinigameIndex
{
    public event Action OnMiniGameFinished;

    [Header("Animals components")]
    public List<AnimalDataSO> animals;
    public List<Button> characteristicSpritesButtons;
    public TextMeshProUGUI descriptionText;
    public GameObject descriptionBox;

    [Header("DialogueManager components")]
    public DialogueManager _dialogueManager;
    public string[] introductionLines;
    public string endGameLine;
    public Button nextDialogueLineButton;
    public Image keyboardInputSprite;

    [Header("Audio")]
    public AudioSource sfxAudioSource;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private int _currentRound = 0;
    private AnimalDataSO _currentAnimal;
    private bool _isCorrect = false;
    private int _minigameIndex;
    private bool _isRandomized => _minigameIndex > 0;
    private bool _waitingForInput = false;

    void Start()
    {
        nextDialogueLineButton.onClick.AddListener(OnNextDialogeLineInput);

        foreach (var btn in characteristicSpritesButtons)
        {
            btn.onClick.RemoveAllListeners();
        }
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
        SetSpritesActive(false);

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
        SetSpritesActive(true);
        StartNextRound();
    }

    private void SetSpritesActive(bool active)
    {
        descriptionBox.SetActive(active);
        for (int i = 0; i < characteristicSpritesButtons.Count; i++)
        {
            characteristicSpritesButtons[i].gameObject.SetActive(active);
        }
    }

    public void StartMiniGame()
    {
        _currentRound = 0;
        sfxAudioSource.Stop();
        CancelInvoke();
        sfxAudioSource.clip = null;
        StartCoroutine(ShowIntroductionDialogue());
    }

    private void StartNextRound()
    {
        if (_currentRound >= animals.Count)
        {
            ShowDialogue("Narrador", endGameLine);
            Invoke(nameof(FinishMiniGame), 3f);
            return;
        }

        if (_isRandomized)
        {
            ShuffleList(animals);
            ShuffleList(characteristicSpritesButtons);
        }

        _currentAnimal = animals[_currentRound];
        _isCorrect = false;

        for (int i = 0; i < characteristicSpritesButtons.Count; i++)
        {
            var button = characteristicSpritesButtons[i];
            var animal = animals[i];

            button.gameObject.SetActive(true);
            button.interactable = true;

            button.GetComponent<Image>().sprite = animal.animalSprite;

            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCharacteristicSelected(animals[index]));
        }

        ShowDescription();
    }

    private void ShowDescription()
    {
        string desc = _currentAnimal.animalDescription;
        descriptionText.text = desc;
    }

    private void OnCharacteristicSelected(AnimalDataSO selected)
    {
        if (_isCorrect) return;

        if (selected == _currentAnimal)
        {
            sfxAudioSource.clip = correctSFX;
            ShowDialogue("Tim", $"¡Encontré al {_currentAnimal.animalName}!");
            _isCorrect = true;
            nextDialogueLineButton.gameObject.SetActive(true);
            _currentRound++;
            Invoke(nameof(StartNextRound), 2f);
        }
        else
        {
            sfxAudioSource.clip = wrongSFX;
            ShowDialogue("Narrador", "No es correcto, ¡intenta con otra opción!");
        }

        sfxAudioSource.Play();
    }

    private void ShowDialogue(string narrator, string feedback)
    {
        Dialogue feedbackDialogue = new Dialogue
        {
            name = narrator,
            sentences = new string[] { feedback }
        };

        _dialogueManager.StartDialogue(feedbackDialogue, null);

        CancelInvoke(nameof(HideDialogue));
        Invoke(nameof(HideDialogue), 2f);
    }

    private void HideDialogue()
    {
        if (_dialogueManager.isDialogueActive)
        {
            _dialogueManager.EndDialogue();
        }
    }

    public void FinishMiniGame()
    {
        OnMiniGameFinished?.Invoke();
    }

    public void SetupMinigame(int minigameIndex)
    {
        _minigameIndex = minigameIndex;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
