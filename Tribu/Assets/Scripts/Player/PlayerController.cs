using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public float speed = 1f;
    private Vector3 targetPosition;
    private bool isMobilePlatform;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject cameraPrefab;
    [SerializeField] public bool isInputEnabled = true;

    NetworkVariable<int> playerscore = new NetworkVariable<int>();

    void Start()
    {
        isMobilePlatform = Application.isMobilePlatform;

        if (isMobilePlatform)
        {
            targetPosition = transform.position;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (cameraPrefab != null && Camera.main == null)
        {
            GameObject cameraInstance = Instantiate(cameraPrefab);
            cameraInstance.GetComponent<Camera>().transform.SetParent(transform);
            cameraInstance.GetComponent<Camera>().transform.localPosition = new Vector3(0, 0, -10);
        }
    }

    void Update()
    {
        if (!isInputEnabled) return;
        if (!IsOwner) return;
        HandlePCControls();
    }

    void HandlePCControls()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
        transform.position += movement * speed * Time.deltaTime;

        animator.SetFloat("Horizontal", moveHorizontal);
        animator.SetFloat("Vertical", moveVertical);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (moveHorizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveHorizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
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
