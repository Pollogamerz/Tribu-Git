using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberQuiz : MonoBehaviour, IStartable, IMinigameIndex
{
    public event Action OnMiniGameFinished;

    [Header("Numbers components")]
    public int maximumRounds;
    public List<NumberDataSO> numbers;
    public List<Button> lilyPadsButtons;
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
    private NumberDataSO _currentNumber;
    private bool _isCorrect = false;
    private int _minigameIndex;
    private bool _isRandomized => _minigameIndex > 0;
    private bool _waitingForInput = false;

    void Start()
    {
        nextDialogueLineButton.onClick.AddListener(OnNextDialogeLineInput);

        foreach (var btn in lilyPadsButtons)
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

    public void StartMiniGame()
    {
        _currentRound = 0;
        sfxAudioSource.Stop();
        CancelInvoke();
        sfxAudioSource.clip = null;
        StartCoroutine(ShowIntroductionDialogue());
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
        for (int i = 0; i < lilyPadsButtons.Count; i++)
        {
            lilyPadsButtons[i].gameObject.SetActive(active);
        }
    }

    private void StartNextRound()
    {
        if (_currentRound >= maximumRounds)
        {
            ShowDialogue("Narrador", endGameLine);
            Invoke(nameof(FinishMiniGame), 3f);
            return;
        }

        _currentNumber = numbers[_currentRound];
        _isCorrect = false;

        for (int i = 0; i < lilyPadsButtons.Count; i++)
        {
            var button = lilyPadsButtons[i];;

            button.gameObject.SetActive(true);
            button.interactable = true;

            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCharacteristicSelected(numbers[index]));
        }

        ShowDescription();
    }

    private void ShowDescription()
    {
        string desc = _currentNumber.description;
        descriptionText.text = desc;
    }

    private void OnCharacteristicSelected(NumberDataSO selected)
    {
        if (_isCorrect) return;

        if (selected == _currentNumber)
        {
            sfxAudioSource.clip = correctSFX;
            ShowDialogue("Narrador", $"¡Bien hecho!");
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

    public void SetupMinigame(int minigameIndex) { }
}
