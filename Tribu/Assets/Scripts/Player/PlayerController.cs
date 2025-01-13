using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public float speed = 1f;
    private Vector3 targetPosition; // Posición objetivo del jugador
    private bool isMoving = false;  // Indica si el jugador está en movimiento
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject cameraPrefab;
    [SerializeField] private NetworkVariable<bool> isInputEnabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> playerScaleX = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> movementInput = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Owner);

    private PlayerInputActions playerInput; // Instancia del esquema de entradas
    private bool isMobilePlatform;

    void Awake()
    {
        // Inicializa el esquema de entrada
        playerInput = new PlayerInputActions();
        playerInput.Enable(); // Activa las entradas
    }

    void Start()
    {
        CheckPlatform();

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

        playerScaleX.OnValueChanged += UpdatePlayerScale;
    }

    void Update()
    {
        if (!IsOwner || !isInputEnabled.Value) return;

        if (isMobilePlatform)
        {
            HandleMobileTouch(); // Manejo de entrada táctil
        }
        else
        {
            HandlePCControls(); // Manejo de teclado para PC
        }

        MoveTowardsTarget(); // Movimiento hacia el objetivo (aplica en móvil)
        UpdateServerMovement();
    }

    void CheckPlatform()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("Estás jugando en un navegador web.");
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Estás jugando en un dispositivo móvil.");
        }
        else
        {
            Debug.Log("Estás jugando en otra plataforma.");
        }
    }

    void HandlePCControls()
    {
        // Obtiene el movimiento desde el sistema de entrada para PC
        Vector2 input = playerInput.Player.Move.ReadValue<Vector2>();
        ApplyMovement(input);
    }

    void HandleMobileTouch()
    {
        // Captura la posición del toque
        Vector2 touchPosition = playerInput.Mobile.Move.ReadValue<Vector2>();

        if (playerInput.Mobile.TouchPress.ReadValue<float>() > 0) // Verifica si se presionó la pantalla
        {
            // Convierte las coordenadas de la pantalla a coordenadas del mundo
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
            targetPosition = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
            isMoving = true; // Marca que el jugador está en movimiento
        }
    }

    void MoveTowardsTarget()
    {
        if (isMoving)
        {
            // Mueve al jugador hacia el objetivo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Calcula la dirección y actualiza las animaciones
            Vector3 direction = targetPosition - transform.position;
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Speed", direction.sqrMagnitude);

            // Detiene el movimiento cuando alcanza el objetivo
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    void ApplyMovement(Vector2 input)
    {
        Vector3 movement = new Vector3(input.x, input.y, 0f);

        transform.position += movement * speed * Time.deltaTime;
        animator.SetFloat("Horizontal", input.x);
        animator.SetFloat("Vertical", input.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (input.x < 0 && transform.localScale.x > 0)
        {
            playerScaleX.Value = -1;
        }
        else if (input.x > 0 && transform.localScale.x < 0)
        {
            playerScaleX.Value = 1;
        }
    }

    void UpdateServerMovement()
    {
        // Actualiza la variable de red con el movimiento del cliente
        movementInput.Value = isMobilePlatform
            ? playerInput.Mobile.Move.ReadValue<Vector2>()
            : playerInput.Player.Move.ReadValue<Vector2>();
    }

    void UpdatePlayerScale(float oldScaleX, float newScaleX)
    {
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
    }

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled.Value = enabled;
    }
}
