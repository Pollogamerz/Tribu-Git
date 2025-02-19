using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class DraggableObject : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform objectTransform;  // Usamos Transform en vez de RectTransform
    private Vector3 startPosition;
    private DropZone dropZone;
    private bool isDragging = false;

    void Start()
    {
        objectTransform = GetComponent<Transform>(); // Obtener el Transform
        startPosition = objectTransform.position;  // Guardar la posición inicial
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsOwner) return; // Solo el dueño del objeto puede arrastrarlo

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsOwner) return;

        // Convertir la posición del mouse en posición del mundo 2D
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = 0; // Mantener en el mismo plano 2D
        objectTransform.position = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsOwner) return;

        isDragging = false;

        // Si se suelta dentro de una DropZone, lo coloca ahí; si no, vuelve a la posición inicial
        if (dropZone != null)
        {
            objectTransform.position = dropZone.transform.position;
        }
        else
        {
            objectTransform.position = startPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DropZone"))
        {
            dropZone = collision.GetComponent<DropZone>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DropZone"))
        {
            dropZone = null;
        }
    }
}