using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform[] patrolPoints;
    public float detectionRange = 5f;

    private Transform player;
    private Animator animator;
    private int currentPatrolIndex = 0;
    private bool isChasingPlayer = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isChasingPlayer && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                ChasePlayer();
            }
            else
            {
                isChasingPlayer = false;
            }
        }
        else
        {
            Patrol();
            DetectPlayer();
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

    private void DetectPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                isChasingPlayer = true;
            }
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            MoveTowards(player.position);
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
