using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Movimiento")]
    [SerializeField] public float speed = 1f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    private bool isRunning = false;

    [Header("Componentes")]
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject cameraPrefab;
    private Rigidbody2D rb;
    private PlayerInputActions playerInput;

    [Header("Red")]
    [SerializeField] private NetworkVariable<bool> isInputEnabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> playerScaleX = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> movementInput = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Owner);

    private bool isMobilePlatform;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Vector2 movementInputRaw;

    void Awake()
    {
        playerInput = new PlayerInputActions();
        playerInput.Enable();
    }

    void Start()
    {
        CheckPlatform();

        if (isMobilePlatform)
            targetPosition = transform.position;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (cameraPrefab != null && Camera.main == null)
        {
            GameObject cam = Instantiate(cameraPrefab);
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0, 0, -10);
        }

        rb = GetComponent<Rigidbody2D>();
        playerScaleX.OnValueChanged += UpdatePlayerScale;
    }

    void Update()
    {
        if (!IsOwner || !isInputEnabled.Value) return;

        if (isMobilePlatform)
        {
            HandleMobileTouch();
        }
        else
        {
            HandleRunInput();
            HandlePCControls();
        }

        UpdateServerMovement();
        MoveTowardsTarget();

        // Movimiento físico con rb
        Vector2 move = movementInputRaw * speed * Time.deltaTime;
        rb.MovePosition(rb.position + move);
    }

    void HandleRunInput()
    {
        if (playerInput.Player.Run.IsPressed())
        {
            if (!isRunning)
            {
                speed *= runSpeedMultiplier;
                isRunning = true;
            }
        }
        else
        {
            if (isRunning)
            {
                speed /= runSpeedMultiplier;
                isRunning = false;
            }
        }
    }

    void HandlePCControls()
    {
        Vector2 input = playerInput.Player.Move.ReadValue<Vector2>();
        movementInputRaw = input;
        ApplyMovement(input);
    }

    void ApplyMovement(Vector2 input)
    {
        animator.SetFloat("Horizontal", input.x);
        animator.SetFloat("Vertical", input.y);
        animator.SetFloat("Speed", input.sqrMagnitude);

        if (input.x < 0 && transform.localScale.x > 0)
            playerScaleX.Value = -1;
        else if (input.x > 0 && transform.localScale.x < 0)
            playerScaleX.Value = 1;
    }

    void HandleMobileTouch()
    {
        Vector2 touchPos = playerInput.Mobile.Move.ReadValue<Vector2>();

        if (playerInput.Mobile.TouchPress.ReadValue<float>() > 0)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, Camera.main.nearClipPlane));
            targetPosition = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            isMoving = true;
        }
    }

    void MoveTowardsTarget()
    {
        if (isMobilePlatform && isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            Vector3 dir = targetPosition - transform.position;

            animator.SetFloat("Horizontal", dir.x);
            animator.SetFloat("Vertical", dir.y);
            animator.SetFloat("Speed", dir.sqrMagnitude);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    void UpdateServerMovement()
    {
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

    void CheckPlatform()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            Debug.Log("Estás jugando en un navegador web.");
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            Debug.Log("Estás jugando en un dispositivo móvil.");
        else
            Debug.Log("Estás jugando en otra plataforma.");
    }
}