using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropFunction : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging;
    private Vector3 posicionInicial;
    private bool sobreZonaValida = false;

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void OnMouseDown()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
        isDragging = true;

        ColorMinigameManager.Instance._currentColorChoose = gameObject.GetComponent<Cubeta>()._colorCubeta;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (sobreZonaValida)
        {
            ColorMinigameManager.Instance.CheckColors();
            transform.position = posicionInicial;
        }
        else
        {
            transform.position = posicionInicial;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DropZone"))
        {
            sobreZonaValida = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DropZone"))
        {
            sobreZonaValida = false;
        }
    }

}
