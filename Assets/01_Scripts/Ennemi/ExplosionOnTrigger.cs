using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.Netcode;
using UnityEngine;

public class ExplosionOnTrigger : NetworkBehaviour
{
    public float explosionForce = 10.0f;
    public float explosionRadius = 5.0f;
    public float upwardsModifier = 3.0f;


    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.transform.GetComponent<PlayerNetworkHandler>())
        {
            Explode(other.GetComponent<HealthComponent>()); Debug.Log("ouiiii");
        }
    }

    void Explode(HealthComponent player)
    {
        //Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        //foreach (Collider hit in colliders)
        //{
        //    Rigidbody rb = hit.GetComponent<Rigidbody>();

        //    if (rb != null)
        //    {
        //        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
        //    }
        //}

        // Ajoutez ici le code pour les effets visuels ou sonores de l'explosion, si nécessaire.

        // Détruit l'objet après l'explosion
        player.TakeDamageClientRpc(6f);

        gameObject.GetComponentInParent<NetworkObject>().Despawn();
    }
}
