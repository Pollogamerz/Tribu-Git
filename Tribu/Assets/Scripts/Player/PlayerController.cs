using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public float speed = 1f;
    private Vector3 targetPosition;
    private bool isMobilePlatform;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject cameraPrefab;
    [SerializeField] private NetworkVariable<bool> isInputEnabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> playerScaleX = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector2> movementInput = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Owner);

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
        playerScaleX.OnValueChanged += UpdatePlayerScale;
    }

    void Update()
    {
        if (!IsOwner || !isInputEnabled.Value) return;

        HandlePCControls();
        UpdateServerMovement();
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
        if (moveHorizontal < 0 && transform.localScale.x > 0)
        {
            playerScaleX.Value = -1;
        }
        else if (moveHorizontal > 0 && transform.localScale.x < 0)
        {
            playerScaleX.Value = 1;
        }
    }

    void UpdateServerMovement()
    {
        movementInput.Value = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void HandleRemotePlayerAnimations()
    {
        Vector2 remoteInput = movementInput.Value;

        animator.SetFloat("Horizontal", remoteInput.x);
        animator.SetFloat("Vertical", remoteInput.y);
        animator.SetFloat("Speed", remoteInput.sqrMagnitude);
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