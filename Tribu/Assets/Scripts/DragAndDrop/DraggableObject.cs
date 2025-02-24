using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class DraggableObject : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform objectTransform;
    private Vector3 startPosition;
    private DropZone dropZone;
    private bool isDragging = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        objectTransform = transform;
        startPosition = objectTransform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsOwner) return;
        isDragging = true;
        ChangeTransparency(0.6f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsOwner) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = 0;
        objectTransform.position = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsOwner) return;
        isDragging = false;
        ChangeTransparency(1f);

        if (IsServer)
        {
            CheckDropPositionServerRpc(objectTransform.position);
        }
        else
        {
            CheckDropPositionServerRpc(objectTransform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DropZone"))
        {
            dropZone = collision.GetComponent<DropZone>();
            Debug.Log("Entra a DropZone: " + dropZone.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DropZone"))
        {
            dropZone = null;
            Debug.Log("Sale de DropZone");
        }
    }


    private void ChangeTransparency(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
        }
    }

    [ServerRpc]
    public void CheckDropPositionServerRpc(Vector3 dropPosition)
    {
        bool isValid = false;

        if (dropZone != null)
        {
            Collider2D dropCollider = dropZone.GetComponent<Collider2D>();

            if (dropCollider != null && dropCollider.bounds.Contains(dropPosition))
            {
                dropPosition = dropCollider.bounds.center; // Centra el objeto en la zona
                isValid = true;
                Debug.Log(" Objeto soltado dentro de DropZone.");
            }
            else
            {
                Debug.Log("⚠El objeto no está completamente dentro de la DropZone.");
            }
        }

        if (!isValid)
        {
            dropPosition = startPosition;
            ShowErrorEffectClientRpc();
            Debug.Log("No es DropZone, regresando a la posición inicial.");
        }

        UpdatePositionClientRpc(dropPosition);
    }




    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        Debug.Log("Actualizando posición a: " + newPosition);
        objectTransform.position = newPosition;
    }

    [ClientRpc]
    private void ShowErrorEffectClientRpc()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke(nameof(ResetColor), 0.5f);
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}
