using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 targetPosition;
    private bool isMobilePlatform;
    public Animator animator;

    void Start()
    {
        isMobilePlatform = Application.isMobilePlatform;

        if (isMobilePlatform)
        {
            targetPosition = transform.position;
        }
    }

    void Update()
    {
        if (isMobilePlatform)
        {
            HandleMobileControls();
        }
        else
        {
            HandlePCControls();
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(targetPosition.magnitude));
        }
    }

    void HandlePCControls()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
        transform.position += movement * speed * Time.deltaTime;
        if (animator != null)
        {
            animator.SetFloat("Horizontal", moveHorizontal);
            animator.SetFloat("Vertical", moveVertical);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    void HandleMobileControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (animator != null)
        {
            Vector3 movement = targetPosition - transform.position;
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }
}