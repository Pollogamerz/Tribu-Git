using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPickup : MonoBehaviour
{
    private bool isHeld = false;

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void PickUp(Transform carryTarget)
    {
        isHeld = true;
        rb.isKinematic = true;
        col.enabled = false;
        transform.SetParent(carryTarget);
        transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        isHeld = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
    }

    public bool IsHeld()
    {
        return isHeld;
    }
}