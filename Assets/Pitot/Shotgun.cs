using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public int bulletNumber = 8;
    public float spreadAngle = 30f;

    public Transform canonPos;

    public void Update()
    {
        for (int i = 0; i < bulletNumber; i++)
        {
            // Générer une direction aléatoire dans un cône
            Vector3 spreadDirection = Random.insideUnitCircle * spreadAngle;
            Vector3 direction = transform.forward + new Vector3(spreadDirection.x, spreadDirection.y, 0f);

            // Lancer un raycast dans la direction définie
            if (Physics.Raycast(canonPos.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                // Si le raycast frappe un objet, effectuez les actions nécessaires (dégâts, effets, etc.)
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
            }
        }
    }
}
