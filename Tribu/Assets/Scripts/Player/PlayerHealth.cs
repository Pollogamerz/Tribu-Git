using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
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
    }
}
