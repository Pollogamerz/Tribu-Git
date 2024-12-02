using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyAttack : NetworkBehaviour
{
    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackCooldown = 1f;

    private bool isAttacking = false;
    private Transform targetPlayer;

    void Update()
    {
        if (!IsServer) return;

        if (targetPlayer != null && !isAttacking)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(PerformAttack());
            }
        }
        else
        {
            DetectPlayers();
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Ejecuta animación de ataque
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackCooldown);

        DealDamageToPlayer();

        isAttacking = false;
    }

    private void DealDamageToPlayer()
    {
        if (targetPlayer != null && Vector2.Distance(transform.position, targetPlayer.position) <= attackRange)
        {
            if (targetPlayer.TryGetComponent(out PlayerHealth playerHealth) && IsServer)
            {
                playerHealth.TakeDamageServerRpc(damage);
            }
        }
    }

    private void DetectPlayers()
    {
        float shortestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject playerObject = client.PlayerObject.gameObject;
            float distance = Vector2.Distance(transform.position, playerObject.transform.position);

            if (distance < attackRange && distance < shortestDistance)
            {
                shortestDistance = distance;
                closestPlayer = playerObject.transform;
            }
        }

        targetPlayer = closestPlayer;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
