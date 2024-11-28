using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackCooldown = 1f;

    private bool isAttacking = false;
    private Transform player;
    private Animator animator;
    private PlayerHealth playerHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player != null && !isAttacking)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
    public void DealDamageToPlayer()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            if (player.TryGetComponent(out playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
