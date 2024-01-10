using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : MonoBehaviour
{

    public float maxRaycastDistance = 100f;

    public void Update()
    {
        // Faire le tir du sniper avec un raycast orienté vers le haut (90 degrés)
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.up * maxRaycastDistance, Color.red, 0.5f);
        
        if (Input.GetMouseButtonDown(0)) // Vérifier si le bouton de la souris (clic gauche) est enfoncé
        {
            if (Physics.Raycast(transform.position, transform.up, out hit, maxRaycastDistance))
            {
                Debug.Log("Sniper shot hit: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Sniper shot missed!");
            }
        }
    }
}
