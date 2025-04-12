using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimalSoundQuiz : MonoBehaviour
{
    [Header("Animals' components")]
    public List<AnimalData> animals;
    public List<Button> animalButtons;

    [Header("UI Buttons")]
    public Button startButton;
    public Button replaySoundButton;
    public Button playAgainButton;
    public Button goToNextLevelButton;

    [Header("Audio components")]
    public AudioSource animalsAudioSource;
    public AudioSource sfxAudioSource;
    [SerializeField] private AudioClip _correctAudio, _incorrectAudio;

    [Header("Rounds")]
    [SerializeField] private int _maximumRounds = 5;

    [Header("Next Level")]
    [SerializeField] private string _nextSceneName;

    private int _currentRound = 0;
    private AnimalData _currentAnimalData;
    private DialogueManager _dialogueManager;
    private bool _isCorrect = false;

    private void Start()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        startButton.onClick.AddListener(StartMiniGame);
        replaySoundButton.onClick.AddListener(ReplaySound);
        replaySoundButton.gameObject.SetActive(false);
        playAgainButton.onClick.AddListener(PlayAgain);
        playAgainButton.gameObject.SetActive(false);
        goToNextLevelButton.onClick.AddListener(LoadNextScene);
        goToNextLevelButton.gameObject.SetActive(false);

        for(int i = 0; i < animalButtons.Count; i++)
        {
            int index = i;
            animalButtons[i].onClick.AddListener(() => OnAnimalSelected(animals[index]));
        }       
    }

    private void StartMiniGame()
    {
        startButton.gameObject.SetActive(false);
        Invoke(nameof(StartNextRound), 2f);
    }

    private void StartNextRound()
    {
        if(_currentRound >= _maximumRounds)
        {
            ShowDialogue("Narrador", "¡Juego terminado!");
            playAgainButton.gameObject.SetActive(true);
            goToNextLevelButton.gameObject.SetActive(true);
            return;
        }

        _currentAnimalData = animals[_currentRound];
        _isCorrect = false;

        animalsAudioSource.clip = _currentAnimalData.animalSound;
        animalsAudioSource.Play();
        replaySoundButton.gameObject.SetActive(true);

        foreach (var button in animalButtons) 
        {
            button.interactable = true;
        }
    }

    private void OnAnimalSelected(AnimalData animalSelected)
    {
        if (animalSelected == _currentAnimalData)
        {
            sfxAudioSource.clip = _correctAudio;
            replaySoundButton.gameObject.SetActive(false);
            ShowDialogue("Narrador", "¡Correcto!");
            _isCorrect = true;
            _currentRound++;
            Invoke(nameof(StartNextRound), 2f);
        }
        else
        {
            sfxAudioSource.clip = _incorrectAudio;
            ShowDialogue("Narrador", "Vuelve a intentarlo");
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

    private void ReplaySound()
    {
        animalsAudioSource.Play();
    }

    private void HideDialogue()
    {
        if (_dialogueManager.isDialogueActive)
        {   
            _dialogueManager.EndDialogue();
        }
    }

    private void PlayAgain()
    {
        _currentRound = 0; 
        playAgainButton.gameObject.SetActive(false);
        goToNextLevelButton.gameObject.SetActive(false);
        Invoke(nameof(StartMiniGame), 2f);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}

[Serializable]
public class AnimalData
{
    public string animalName;
    public AudioClip animalSound;
    public Sprite animalSprite;
}
