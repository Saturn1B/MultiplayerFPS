using UnityEngine;

public class Shotgun : CustomWeapon
{
    public int bulletNumber = 30; // Nombre de projectiles
    public float spreadAngle = 7f; // Angle de dispersion
    public int ralspread;
    public float maxRaycastDistance = 25f;

    protected override void Shoot()
	{
        /*for (int i = 0; i < bulletNumber; i++)
        {
            // Générer une direction aléatoire dans un cône
            Vector3 direction = new Vector3(Random.Range(-spreadAngle, spreadAngle) * Mathf.Cos(Random.Range(0, 2 * Mathf.PI)), Random.Range(-spreadAngle, spreadAngle) * Mathf.Sin(Random.Range(0, 2 * Mathf.PI)), ralspread);

            // Lancer un raycast dans la direction définie
            RaycastHit hit;
            if (Physics.Raycast(startPoint.position, direction, out hit, maxRaycastDistance))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                Hit(hit);
            }
            else
            {
                Debug.Log("Missed!");
            }

            // Affichage visuel du raycast
            Debug.DrawRay(startPoint.position, direction * maxRaycastDistance, Color.red, 3f);
        }*/
    }
}
