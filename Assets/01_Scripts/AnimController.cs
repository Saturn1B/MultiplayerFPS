using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimController : NetworkBehaviour
{
    private Animator animator; // R�f�rence vers l'Animator
    public GameObject skinnedMeshRenderer;

    void Start()
    {
        // Obtention de la r�f�rence de l'Animator
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        //skinnedMeshRenderer.SetActive(false);
        // V�rifie si la touche W est press�e       

        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.Play("Forward");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.Play("Left");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.Play("Backward");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.Play("Right");
        }

        if (!Input.anyKey)
        {
            // Aucune touche n'est press�e, tu peux mettre ici le code que tu veux ex�cuter
            animator.Play("Idle");
        }


    }
}


