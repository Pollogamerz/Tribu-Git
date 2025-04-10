using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalSoundQuiz : MonoBehaviour
{
    public List<AnimalData> animals;
    public List<Button> animalButtons;
    public AudioSource audioSource;
    public Button replaySoundButton;

    [SerializeField] private int _maximumRounds = 5;

    private int _currentRound = 0;
    private AnimalData _currentAnimalData;
    private DialogueManager _dialogueManager;
    private bool _isCorrect = false;

    private void Start()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();

        replaySoundButton.onClick.AddListener(ReplaySound);
        replaySoundButton.gameObject.SetActive(false);

        for(int i = 0; i < animalButtons.Count; i++)
        {
            int index = i;
            animalButtons[i].onClick.AddListener(() => OnAnimalSelected(animals[index]));
        }

        StartNextRound();
    }

    private void StartNextRound()
    {
        if(_currentRound >= _maximumRounds)
        {
            ShowDialogue("Narrador", "¡Juego terminado!");
            return;
        }

        _currentAnimalData = animals[_currentRound];
        _isCorrect = false;

        audioSource.clip = _currentAnimalData.animalSound;
        audioSource.Play();
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
            replaySoundButton.gameObject.SetActive(false);
            ShowDialogue("Narrador", "¡Correcto!");
            _isCorrect = true;
            _currentRound++;
            Invoke(nameof(StartNextRound), 2f);
        }
        else
        {
            ShowDialogue("Narrador", "Vuelve a intentarlo");
        }
    }

    private void ShowDialogue(string narrator, string feedback)
    {
        Dialogue feedbackDialogue = new Dialogue
        {
            name = narrator,
            sentences = new string[] { feedback }
        };
        _dialogueManager.StartDialogue(feedbackDialogue, null);
    }

    private void ReplaySound()
    {
        audioSource.Play();
    }
}

[Serializable]
public class AnimalData
{
    public string animalName;
    public AudioClip animalSound;
    public Sprite animalSprite;
}
