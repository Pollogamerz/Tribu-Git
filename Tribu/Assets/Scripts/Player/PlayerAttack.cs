using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 0.5f;
    public int damage = 10;
    public float attackRange = 1f;
    private bool isAttacking = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PerformAttack();
        }
    }

    public void PerformAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(HandleAttack());
        }
    }

    private IEnumerator HandleAttack()
    {
        isAttacking = true;
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
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
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
