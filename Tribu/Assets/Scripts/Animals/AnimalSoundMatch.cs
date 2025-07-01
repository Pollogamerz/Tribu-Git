using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalSoundMatch : MonoBehaviour, IStartable, IMinigameIndex
{
    public List<AnimalDataSO> animals;
    [SerializeField] private int _maximumRounds = 3;

    [Header("Introduction dialogue")]
    public string[] introductionLines;

    [Header("UI Components")]
    public Image animalSprite;
    public Button leftSoundButton;
    public Button rightSoundButton;
    public Button leftAnswerButton;
    public Button rightAnswerButton;
    public Button nextDialogueLineButton;
    public Image keyboardInputSprite;

    [Header("Audio Components")]
    public AudioSource animalsAudioSource;
    public AudioSource sfxAudioSource;
    [SerializeField] private AudioClip _correctAudio, _incorrectAudio;

    private int _currentRound = 0;
    public DialogueManager _dialogueManager;
    private AnimalDataSO _currentAnimalData;
    private AudioClip _leftClip;
    private AudioClip _rightClip;
    private bool _isCorrectSound;
    private bool _waitingForInput = false;

    public event System.Action OnMiniGameFinished;

    private void Start()
    {
        leftSoundButton.onClick.AddListener(() => PlaySound(_leftClip));
        rightSoundButton.onClick.AddListener(() => PlaySound(_rightClip));
        leftAnswerButton.onClick.AddListener(() => OnAnswerSelected(_isCorrectSound));
        rightAnswerButton.onClick.AddListener(() => OnAnswerSelected(!_isCorrectSound));
        nextDialogueLineButton.onClick.AddListener(OnNextDialogueLineInput);
    }

    public void StartMiniGame()
    {
        animalsAudioSource.Stop();
        sfxAudioSource.Stop();
        CancelInvoke();
        animalsAudioSource.clip = null;
        sfxAudioSource.clip = null;
        _currentRound = 0;
        StartCoroutine(ShowIntroductionDialogue());
    }

    private void Update()
    {
        if (_waitingForInput && Input.GetKeyDown(KeyCode.E)) 
        { 
            _waitingForInput = false;
        }
    }

    public void OnNextDialogueLineInput()
    {
        _waitingForInput = false;
    }

    private IEnumerator ShowIntroductionDialogue()
    {
        nextDialogueLineButton.interactable = true;
        keyboardInputSprite.gameObject.SetActive(true);
        animalSprite.gameObject.SetActive(false);
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
        animalSprite.gameObject.SetActive(true);
        SetButtonsInteractable(true);
        StartNextRound();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        leftAnswerButton.interactable = interactable;
        rightAnswerButton.interactable = interactable;
        leftSoundButton.interactable = interactable;
        rightSoundButton.interactable = interactable;
    }

    private void StartNextRound()
    {
        if (_currentRound >= _maximumRounds)
        {
            ShowDialogue("Narrador", "¡Juego terminado!");
            Invoke(nameof(FinishMiniGame), 2f);
            return;
        }

        SetButtonsInteractable(true);

        _currentAnimalData = animals[_currentRound];
        animalSprite.sprite = _currentAnimalData.animalSprite;

        List<AudioClip> clip = new List<AudioClip>();

        foreach (var animal in animals)
        {
            if (animal.animalSound != _currentAnimalData.animalSound)
            {
                clip.Add(animal.animalSound);
            }
        }

        AudioClip wrongClip = clip.Count > 0 ? clip[Random.Range(0, clip.Count)] : _currentAnimalData.animalSound;

        _isCorrectSound = Random.value < 0.5f;
        _leftClip = _isCorrectSound ? _currentAnimalData.animalSound : wrongClip;
        _rightClip = _isCorrectSound ? wrongClip : _currentAnimalData.animalSound;
    }

    private void PlaySound(AudioClip clip)
    {
        animalsAudioSource.clip = clip;
        animalsAudioSource.Play();
    }

    private void OnAnswerSelected(bool isCorrect)
    {
        if (isCorrect)
        {
            sfxAudioSource.clip = _correctAudio;
            ShowDialogue("Narrator", $"Correcto! Es un {_currentAnimalData.animalName}");
            _currentRound++;
            SetButtonsInteractable(false);
            Invoke(nameof(StartNextRound), 2f);
        }
        else
        {
            ShowDialogue("Narrator", "Intenta otra vez!");
            sfxAudioSource.clip = _incorrectAudio;
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
