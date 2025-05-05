using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropFunction : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging;
    private Vector3 posicionInicial;
    private bool sobreZonaValida = false, mezclador = false;

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
        ColorMinigameManager.Instance._bucket = this.gameObject;
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

        if (mezclador)
        {
            transform.position = posicionInicial;
            if (Mezclador.Instance._firstColor == "")
            {
                Mezclador.Instance._firstColor = gameObject.GetComponent<Cubeta>()._nombreColor;
            }
            else
            {
                Mezclador.Instance._secondColor = gameObject.GetComponent<Cubeta>()._nombreColor;
            }

            if (Mezclador.Instance._firstColor != "" && Mezclador.Instance._secondColor != "")
            {
                Mezclador.Instance.SpawnNewColor();
            }
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

        if (other.CompareTag("Mezclador"))
        {
            mezclador = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DropZone"))
        {
            sobreZonaValida = false;
        }

        if (other.CompareTag("Mezclador"))
        {
            mezclador = false;
        }
    }

}
