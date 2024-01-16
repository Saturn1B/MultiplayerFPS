using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemiAi : MonoBehaviour
{

    public float detectionRadius = 10f;
    public float chaseSpeed = 5f;
    public float returnSpeed = 3f;
    private Transform player;
    private Vector3 initialPosition;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // V�rifie si le joueur est dans le rayon de d�tection
        if (Vector3.Distance(transform.position, player.position) < detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            ReturnToInitialPosition();

        }
    }

    void ChasePlayer()
    {
        navMeshAgent.destination = player.position;
    }

    void ReturnToInitialPosition()
    {
        navMeshAgent.destination = initialPosition;
    }

    void OnDrawGizmos()
    {
        // Dessine une sph�re de d�tection dans l'�diteur Unity pour visualiser la port�e
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
