using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSoundButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private AnimalDataSO data;
    private SoundDragMinigame controller;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;

    public AudioSource audioSource;

    public void SetData(AnimalDataSO animalData, SoundDragMinigame gameController)
    {
        controller = gameController;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.localPosition;
        originalParent = transform.parent;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => PlaySound());
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(data.animalSound);
    }

    public AnimalDataSO GetAnimalData() => data;

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
        rectTransform.localPosition = originalPosition;
    }
}
