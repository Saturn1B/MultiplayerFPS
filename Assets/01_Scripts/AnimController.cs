using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimController : MonoBehaviour
{
    private Animator animator; // Référence vers l'Animator


    void Start()
    {
        // Obtention de la référence de l'Animator
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Vérifie si la touche W est pressée
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.Play("Forward");
        }

        if (!Input.anyKey)
        {
            // Aucune touche n'est pressée, tu peux mettre ici le code que tu veux exécuter
            animator.Play("Idle");
        }


    }
}
