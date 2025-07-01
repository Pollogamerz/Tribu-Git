using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimalSoundQuiz : MonoBehaviour, IStartable, IMinigameIndex
{
    [Header("Animals' components")]
    public List<AnimalDataSO> animals;
    public List<Button> animalButtons;

    [Header("UI Buttons")]
    public Button replaySoundButton;
    public Image dialogueESprite;
    public Button dialogueInputButton;

    [Header("Audio components")]
    public AudioSource animalsAudioSource;
    public AudioSource sfxAudioSource;
    [SerializeField] private AudioClip _correctAudio, _incorrectAudio;

    [Header("Next Level")]
    [SerializeField] private string _nextSceneName;

    public DialogueManager dialogueManager;
    
    private int _currentRound = 0;
    private AnimalDataSO _currentAnimalData;
    private bool _isCorrect = false;

    private int _minigameIndex;
    private bool _isRandomized => _minigameIndex > 0;

    public event Action OnMiniGameFinished;

    private void Start()
    {
        replaySoundButton.onClick.AddListener(ReplaySound);
        replaySoundButton.gameObject.SetActive(false);
        dialogueESprite.gameObject.SetActive(false);
        dialogueInputButton.interactable = false;

        for (int i = 0; i < animalButtons.Count; i++)
        {
            Image buttonImage = animalButtons[i].GetComponent<Image>();
            buttonImage.sprite = animals[i].animalSprite;
            int index = i;
            animalButtons[i].onClick.AddListener(() => OnAnimalSelected(animals[index]));
        }       
    }

    public void StartMiniGame()
    {
        animalsAudioSource.Stop();
        sfxAudioSource.Stop();
        CancelInvoke();
        animalsAudioSource.clip = null;
        sfxAudioSource.clip = null;
        _currentRound = 0;
        Invoke(nameof(StartNextRound), 2f);
    }

    private void StartNextRound()
    {
        if (_currentRound >= animals.Count)
        {
            ShowDialogue("Narrador", "¡Juego terminado!");
            Invoke(nameof(FinishMiniGame), 2f);
            return;
        }

        if (_isRandomized)
        {
            ShuffleList(animals);
            ShuffleList(animalButtons);
        }

        _currentAnimalData = animals[_currentRound];
        _isCorrect = false;

        animalsAudioSource.clip = _currentAnimalData.animalSound;
        animalsAudioSource.Play();
        replaySoundButton.gameObject.SetActive(true);

        for (int i = 0; i < animalButtons.Count; i++)
        {
            Image buttonImage = animalButtons[i].GetComponent<Image>();
            buttonImage.sprite = animals[i].animalSprite;
            int index = i;
            animalButtons[i].onClick.RemoveAllListeners();
            animalButtons[i].onClick.AddListener(() => OnAnimalSelected(animals[index]));
            animalButtons[i].interactable = true;
        }
    }

    private void OnAnimalSelected(AnimalDataSO animalSelected)
    {
        if (animalSelected == _currentAnimalData && _isCorrect != true)
        {
            sfxAudioSource.clip = _correctAudio;
            replaySoundButton.gameObject.SetActive(false);
            ShowDialogue("Narrador", $"¡Correcto! Es el sonido del {_currentAnimalData.animalName}");
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

        dialogueManager.StartDialogue(feedbackDialogue, null);

        CancelInvoke(nameof(HideDialogue));
        Invoke(nameof(HideDialogue), 2f);
    }

    private void ReplaySound()
    {
        animalsAudioSource.Play();
    }

    private void HideDialogue()
    {
        if (dialogueManager.isDialogueActive)
        {   
            dialogueManager.EndDialogue();
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
