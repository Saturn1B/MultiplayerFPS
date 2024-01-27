using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using static UnityEngine.GraphicsBuffer;

public class EnnemiAi : NetworkBehaviour
{
    public bool shooter;
    
    public bool active;

    public float raycastDistance = 10f;
    public float chaseSpeed = 5f;
    public float returnSpeed = 3f;

    [SerializeField]
    private Transform targets;

    private Vector3 initialPosition;
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private bool targetFound = false;
    [SerializeField]
    private bool shoot = false;

    public Transform transformShoot;

    public override void OnNetworkSpawn()
    {      
        navMeshAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
    }

    public void OnTriggerStay(Collider other)
    {
        if (!IsServer) return;
        //Debug.Log("trigg rien");

        if (other.transform.GetComponent<PlayerNetworkHandler>() && active)
        {
            if (other.transform.GetComponent<PlayerNetworkHandler>().isDown.Value)
            {
                //Je fous quoi mdr ^^ player dead
            }
            else
            {
                targets = other.transform;
            }
            

            if (shooter && other.transform.GetComponent<PlayerNetworkHandler>() && other.transform.GetComponent<PlayerNetworkHandler>().isDown.Value == false)
            {
                // Obtient la position actuelle de l'objet
                Vector3 raycastOrigin = transformShoot.position;

                // Obtient la direction du raycast (dans la direction vers l'avant de l'objet)
                Vector3 raycastDirection = transform.forward;

                // Déclaration d'une variable pour stocker les informations du hit du raycast
                RaycastHit hit;

                // Effectue le raycast
                if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, raycastDistance))
                {
                    // Vérifie si l'objet touché par le raycast est le joueur
                    if (hit.transform.GetComponent<PlayerNetworkHandler>())
                    {
                        // Le joueur est détecté, arrête le mouvement et vise le joueur

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

                // Dessine une ligne représentant le raycast dans l'éditeur pour le débogage
                Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, Color.green);
            }
            else if (!shooter && other.transform.GetComponent<PlayerNetworkHandler>() && other.transform.GetComponent<PlayerNetworkHandler>().isDown.Value == false)
            {
                ChasePlayer();
            }

        }

    }

    

    void ChasePlayer()
    {
        navMeshAgent.destination = targets.position;
    }

    void ShootPos()
    {
        if (targetFound)
        {
            // Si la cible est trouvée, arrête le mouvement et vise le joueur
            StopMovementAndAim();
        }
        else
        {
            // Sinon, continue à suivre le joueur
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = targets.position;
        }
    }

    private void StopMovementAndAim()
    {    
        // Arrête le mouvement du NavMeshAgent
        navMeshAgent.isStopped = true;

        if (shoot == false)
        {
            shoot = true;

            StartCoroutine(ShootRoutine());
        }
        
    }

    IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (targetFound)
        {
            Debug.Log("pew pew");

            targets.GetComponent<HealthComponent>().TakeDamageClientRpc(1f);
        }
        else
        {
            Debug.Log("plus d'enemy");
        }

        shoot = false;
    }

}
