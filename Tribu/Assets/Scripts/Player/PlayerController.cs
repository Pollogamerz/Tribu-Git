using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    private Vector3 targetPosition;
    private bool isMobilePlatform;
    public Animator animator;

    public GameObject cameraPrefab; // Prefab de la cámara para el jugador

    public bool isInputEnabled = true; // Control de si el jugador puede moverse o no

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            isInputEnabled = false;  // Desactivar movimiento si no es el propietario
        }

        // Si este jugador es el propietario, crear y asignar la cámara
        if (IsOwner)
        {
            // Instanciar la cámara solo para el propietario
            GameObject cameraInstance = Instantiate(cameraPrefab);
            cameraInstance.GetComponent<Camera>().transform.SetParent(transform);
            cameraInstance.GetComponent<Camera>().transform.localPosition = new Vector3(0, 0, -10); // Ajuste de la cámara

            AudioListener audioListener = cameraInstance.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = true; // Habilitar el audio listener solo en la cámara del propietario
            }
        }
    }

    void Start()
    {
        if (!IsOwner)
        {
            enabled = false; // Desactivar el script para clientes
            return;
        }

        isMobilePlatform = Application.isMobilePlatform;

        if (isMobilePlatform)
        {
            targetPosition = transform.position;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (!IsOwner || !isInputEnabled) return;

        HandlePCControls(); // Lógica de control de movimiento
    }

    void HandlePCControls()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
        transform.position += movement * speed * Time.deltaTime;

        float totalSpeed = new Vector2(moveHorizontal, moveVertical).magnitude;
        animator.SetFloat("Horizontal", moveHorizontal);
        animator.SetFloat("Vertical", moveVertical);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (moveHorizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (moveHorizontal > 0)
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
