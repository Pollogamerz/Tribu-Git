using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] public int maxHealth = 3;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] public Image[] hearts;
    [SerializeField] public Sprite fullHeart;
    [SerializeField] public Sprite emptyHeart;
    [SerializeField] public Transform respawnPoint;
    [SerializeField] public float respawnDelay = 2f;
    [SerializeField] public Animator animator;

    private Canvas healthCanvas;

    void Start()
    {
        if (IsOwner)
        {
            healthCanvas = GetComponentInChildren<Canvas>();
            currentHealth.OnValueChanged += UpdateHeartsUI;
            UpdateHeartsUI(currentHealth.Value, maxHealth);
        }
        else
        {
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Enemy"))
        {
            TakeDamageServerRpc(1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        if (currentHealth.Value > 0)
        {
            currentHealth.Value -= damage;
            TriggerHitAnimationClientRpc();
        }

        if (currentHealth.Value <= 0)
        {
            TriggerDieAnimationClientRpc();
            StartCoroutine(Respawn()); 
        }
    }

    [ClientRpc]
    private void TriggerHitAnimationClientRpc()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    [ClientRpc]
    private void TriggerDieAnimationClientRpc()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    private void UpdateHeartsUI(int oldHealth, int newHealth)
    {
        if (!IsOwner) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < newHealth) ? fullHeart : emptyHeart;
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        currentHealth.Value = maxHealth;
        RespawnClientRpc(respawnPoint.position);
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 respawnPosition)
    {
        transform.position = respawnPosition;

        if (IsOwner)
        {
            UpdateHeartsUI(0, currentHealth.Value);
        }

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.SetTrigger("Respawn");
        }
    }
}