using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotate : MonoBehaviour
{
    public Transform[] shields; // Tableau contenant les trois boucliers
    public float orbitSpeed = 30.0f; // La vitesse de rotation des boucliers
    public float distance = 3.0f; // Distance à laquelle les boucliers tournent autour de l'ennemi

    private Vector3 centerPosition;

    void Start()
    {
        // Calculer la position centrale de l'ennemi
        centerPosition = transform.position;
    }

    void Update()
    {
        // Assure-toi que le tableau de boucliers existe
        if (shields != null && shields.Length >= 3)
        {
            // Faire tourner les trois boucliers autour de l'ennemi
            for (int i = 0; i < 3; i++)
            {
                Vector3 orbitPosition = centerPosition +
                    Quaternion.Euler(0, orbitSpeed * Time.time + (i * 120), 0) * Vector3.forward * distance;

                shields[i].position = orbitPosition;
            }
        }
    }
}
