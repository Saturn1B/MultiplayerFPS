using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimController : MonoBehaviour
{
    private Animator animator; // R�f�rence vers l'Animator


    void Start()
    {
        // Obtention de la r�f�rence de l'Animator
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // V�rifie si la touche W est press�e
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.Play("Forward");
        }

        if (!Input.anyKey)
        {
            // Aucune touche n'est press�e, tu peux mettre ici le code que tu veux ex�cuter
            animator.Play("Idle");
        }


    }
}
