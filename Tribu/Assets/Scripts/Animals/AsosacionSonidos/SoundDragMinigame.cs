using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundDragMinigame : MonoBehaviour, IStartable, IMinigameIndex
{
    [Header("Referencias fijas en escena")]
    public List<DraggableSoundButton> allSoundButtons;
    public List<AnimalDropZone> allDropZones;

    [Header("Datos")]
    public List<AnimalDataSO> allAnimals;
    public int numberOfAnimalsToUse = 3;
    private List<AnimalDataSO> animalsToUse;

    [Header("Audio")]
    public AudioClip successSound;
    public AudioClip errorSound;
    public AudioSource audioSource;

    private int correctMatches;

    public event Action OnMiniGameFinished;

    public void StartMiniGame()
    {
        correctMatches = 0;

        animalsToUse = allAnimals.OrderBy(a => UnityEngine.Random.value).Take(numberOfAnimalsToUse).ToList();

        foreach (var btn in allSoundButtons) btn.gameObject.SetActive(false);
        foreach (var zone in allDropZones) zone.gameObject.SetActive(false);

        List<DraggableSoundButton> shuffledButtons = allSoundButtons
            .OrderBy(x => UnityEngine.Random.value)
            .Take(animalsToUse.Count)
            .ToList();

        for (int i = 0; i < animalsToUse.Count; i++)
        {
            var data = animalsToUse[i];

            var soundButton = shuffledButtons[i];
            soundButton.gameObject.SetActive(true);
            soundButton.SetData(data, this);

            var dropZone = allDropZones[i];
            dropZone.gameObject.SetActive(true);
            dropZone.SetData(data, this);
        }
    }

    public void RegisterCorrectMatch()
    {
        correctMatches++;
        audioSource.PlayOneShot(successSound);

        if (correctMatches >= animalsToUse.Count)
        {
            FinishMiniGame();
        }
    }

    public void RegisterIncorrectMatch()
    {
        audioSource.PlayOneShot(errorSound);
    }

    public void FinishMiniGame()
    {
        OnMiniGameFinished?.Invoke();
    }

    public void SetupMinigame(int minigameIndex) { }
}
