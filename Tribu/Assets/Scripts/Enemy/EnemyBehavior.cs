using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyBehavior : NetworkBehaviour
{
    public float moveSpeed = 2f;
    public Transform[] patrolPoints;
    public float detectionRange = 5f;

    private Transform targetPlayer;
    private Animator animator;
    private int currentPatrolIndex = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsServer) return;

        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer <= detectionRange)
            {
                ChasePlayer();
            }
            else
            {
                targetPlayer = null;
            }
        }
        else
        {
            Patrol();
            DetectPlayers();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        MoveTowards(patrolTarget.position);

        if (Vector2.Distance(transform.position, patrolTarget.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
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

            if (distance < detectionRange && distance < shortestDistance)
            {
                shortestDistance = distance;
                closestPlayer = playerObject.transform;
            }
        }

        if (closestPlayer != null)
        {
            targetPlayer = closestPlayer;
        }
    }

    private void ChasePlayer()
    {
        if (targetPlayer != null)
        {
            MoveTowards(targetPlayer.position);
        }
    }

    private void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Speed", direction.sqrMagnitude);
        }

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
