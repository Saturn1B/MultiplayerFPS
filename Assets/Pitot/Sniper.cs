using UnityEngine;

public class Sniper : CustomWeapon
{
    public float maxRaycastDistance = 100f;

    protected override void Shoot()
    {
        // Faire le tir du sniper avec un raycast orienté vers le haut (90 degrés)
        RaycastHit hit;
        if (Physics.Raycast(startPoint.position, transform.up, out hit, maxRaycastDistance))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            Hit(hit);
        }
        else
        {
            Debug.Log("Missed!");
        }

        Debug.DrawRay(transform.position, transform.up * maxRaycastDistance, Color.red, 0.5f);
    }
}
