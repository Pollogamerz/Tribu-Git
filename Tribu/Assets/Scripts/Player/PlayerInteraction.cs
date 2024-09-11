using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 1.5f;
    public LayerMask interactableLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            CheckInteraction();
        }
    }

    void CheckInteraction()
    {
        Collider2D[] interactables = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
        foreach (Collider2D interactable in interactables)
        {
            interactable.GetComponent<IInteractable>()?.Interact();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    public interface IInteractable
    {
        void Interact();
    }
}