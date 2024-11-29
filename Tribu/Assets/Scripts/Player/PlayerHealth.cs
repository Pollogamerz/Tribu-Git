using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] public Image[] hearts;
    [SerializeField] public Sprite fullHeart;
    [SerializeField] public Sprite emptyHeart;
    [SerializeField] public Transform respawnPoint;
    [SerializeField] public float respawnDelay = 2f;
    [SerializeField] public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    private void Die()
    {
        Debug.Log("Se murio Tim pipipi");
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        GetComponent<PlayerController>().enabled = false;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentHealth = maxHealth;
        UpdateHeartsUI();
        transform.position = respawnPoint.position;
        GetComponent<PlayerController>().enabled = true;
        if (animator != null)
        {
            animator.SetTrigger("Speed");
        }

        Debug.Log("Tim ha vuelto a la vida pipipi");
    }
}
