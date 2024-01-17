using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

public class ExplosionOnTrigger : MonoBehaviour
{
    public float explosionForce = 10.0f;
    public float explosionRadius = 5.0f;
    public float upwardsModifier = 3.0f;

    public Health health;
    public Health playerHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Explode(other.GetComponent<Health>()); Debug.Log("ouiiii");
        }
    }

    void Explode(Health player)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            }
        }

        // Ajoutez ici le code pour les effets visuels ou sonores de l'explosion, si nécessaire.

        // Détruit l'objet après l'explosion
        player.Kill();

        health.Kill();
    }
}
