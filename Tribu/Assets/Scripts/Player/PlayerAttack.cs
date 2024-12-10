using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public float attackCooldown = 0.5f;
    [SerializeField] public int damage = 10;
    [SerializeField] public float attackRange = 1f;
    private bool isAttacking = false;
    public NetworkVariable<bool> isInputEnabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private PlayerInputActions playerInput;

    void Awake()
    {
        playerInput = new PlayerInputActions();
    }

    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (!IsOwner || !isInputEnabled.Value) return;

        // Detectar si se presionó el botón de ataque
        if (playerInput.Player.Attack.WasPressedThisFrame())
        {
            PerformAttack();
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled.Value = enabled;
    }

    public void PerformAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            PerformAttackServerRpc();
            StartCoroutine(ResetAttackCooldown());
        }
    }

    [ServerRpc]
    private void PerformAttackServerRpc()
    {
        DealDamageToEnemies();
        PerformAttackClientRpc();
    }

    [ClientRpc]
    private void PerformAttackClientRpc()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void DealDamageToEnemies()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                var enemyController = enemy.GetComponent<EnemyHealth>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(damage);
                }
            }
        }
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
