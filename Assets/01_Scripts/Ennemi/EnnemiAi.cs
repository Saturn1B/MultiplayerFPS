using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemiAi : MonoBehaviour
{
    public bool shooter;

    public float raycastDistance = 10f;
    public float chaseSpeed = 5f;
    public float returnSpeed = 3f;


    private Transform player;
    private Vector3 initialPosition;
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private bool targetFound = false;

    public Transform transformShoot;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;

        
    }

    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("trigg rien");

        if (other.CompareTag("Player"))
        {
            if (shooter && other.CompareTag("Player"))
            {
                // Obtient la position actuelle de l'objet
                Vector3 raycastOrigin = transformShoot.position;

                // Obtient la direction du raycast (dans la direction vers l'avant de l'objet)
                Vector3 raycastDirection = transform.forward;

                // D�claration d'une variable pour stocker les informations du hit du raycast
                RaycastHit hit;

                // Effectue le raycast
                if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, raycastDistance))
                {
                    // V�rifie si l'objet touch� par le raycast est le joueur
                    if (hit.collider.CompareTag("Player"))
                    {
                        // Le joueur est d�tect�, arr�te le mouvement et vise le joueur
                        targetFound = true;
                        StopMovementAndAim();
                    }
                    else
                    {
                        targetFound = false;
                        ShootPos();
                    }
                }
                else
                {
                    targetFound = false;
                    ShootPos();
                }

                // Dessine une ligne repr�sentant le raycast dans l'�diteur pour le d�bogage
                Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, Color.green);
            }
            else if (!shooter && other.CompareTag("Player"))
            {
                ChasePlayer();
            }

        }

    }

    

    void ChasePlayer()
    {
        navMeshAgent.destination = player.position;
    }

    void ShootPos()
    {
        if (targetFound)
        {
            // Si la cible est trouv�e, arr�te le mouvement et vise le joueur
            StopMovementAndAim();
        }
        else
        {
            // Sinon, continue � suivre le joueur
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = player.position;
        }
    }

    private void StopMovementAndAim()
    {
        // Arr�te le mouvement du NavMeshAgent
        navMeshAgent.isStopped = true;

        // Ajoute le code pour viser le joueur ici (par exemple, faire pivoter l'ennemi vers le joueur)
        // ...

        // Ajoutez ici le code pour effectuer l'action de tir ou autre action li�e � l'arr�t
        // ...
    }

    void ReturnToInitialPosition()
    {
        navMeshAgent.destination = initialPosition;
    }

    /*void OnDrawGizmos()
    {
        // Dessine une sph�re de d�tection dans l'�diteur Unity pour visualiser la port�e
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }*/

}
