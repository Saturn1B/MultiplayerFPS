using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimController : NetworkBehaviour
{
    public Animator animator; // R�f�rence vers l'Animator
    public GameObject skinnedMeshRenderer;
    public GameObject basicSkin;

    void Start()
    {
        if (!IsLocalPlayer) return;

        basicSkin.SetActive(false);
        skinnedMeshRenderer.SetActive(true);
        // Obtention de la r�f�rence de l'Animator
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

     

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


