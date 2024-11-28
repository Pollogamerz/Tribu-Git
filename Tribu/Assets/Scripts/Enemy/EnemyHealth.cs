using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    public Animator animator;
    private GameObject deathEffect;
    void Start()
    {
        currentHealth = maxHealth;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, 1f);
    }
}
