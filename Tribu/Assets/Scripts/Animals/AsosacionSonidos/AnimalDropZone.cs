using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimalDropZone : MonoBehaviour, IDropHandler
{
    public Image animalImage;
    private AnimalDataSO expectedAnimal;
    private bool isMatched = false;
    private SoundDragMinigame controller;

    public void SetData(AnimalDataSO animalData, SoundDragMinigame gameController)
    {
        expectedAnimal = animalData;
        animalImage.sprite = expectedAnimal.animalSprite;
        controller = gameController;
        isMatched = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isMatched) return;

        DraggableSoundButton dropped = eventData.pointerDrag?.GetComponent<DraggableSoundButton>();
        if (dropped != null)
        {
            if (dropped.GetAnimalData() == expectedAnimal)
            {
                isMatched = true;
                controller.RegisterCorrectMatch();
                Destroy(dropped.gameObject);
            }
            else
            {
                controller.RegisterIncorrectMatch();
            }
        }
    }
}
