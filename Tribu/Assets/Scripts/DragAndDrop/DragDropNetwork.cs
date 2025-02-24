using UnityEngine;
using Unity.Netcode;

public class DragDropNetwork : NetworkBehaviour
{
    private Vector3 startPosition;
    private bool isDragging = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (!IsOwner) return; // Solo el dueño puede mover el objeto

        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Posición en el mundo: " + mousePosition);
            mousePosition.z = 0; // Asegurar que el objeto se mantenga en 2D
            transform.position = mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            CheckDropPositionServerRpc(transform.position);
        }
    }

    void TryStartDrag()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null)
        {
            //Debug.Log("Clic en: " + hit.gameObject.name); // Para ver si detecta algo
        }

        if (hit != null && hit.gameObject == gameObject)
        {
            isDragging = true;
            //Debug.Log("Arrastre iniciado en: " + gameObject.name);
        }
    }


    [ServerRpc]
    private void CheckDropPositionServerRpc(Vector3 dropPosition)
    {
        bool isValid = IsValidDrop(dropPosition);

        if (!isValid)
        {
            dropPosition = startPosition; // Si no es válido, regresa a su posición original
        }

        UpdatePositionClientRpc(dropPosition);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    private bool IsValidDrop(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position);
        return hit != null && hit.CompareTag("DropZone"); // Verifica si el objeto se suelta en un destino válido
    }
}
