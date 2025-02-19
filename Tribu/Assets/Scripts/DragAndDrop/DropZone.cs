using UnityEngine;
using Unity.Netcode;

public class DropZone : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Draggable"))
        {
            Debug.Log("Objeto arrastrable dentro de la DropZone");
        }
    }
}