using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public int bulletNumber = 30; // Nombre de projectiles
    public float spreadAngle = 7f; // Angle de dispersion
    public int ralspread;

    public Transform muzzlePoint; // Point de d�part des projectiles

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Tir lors du clic gauche
        {
        FireShotgun();
        }
    }

    void FireShotgun()
    {
        for (int i = 0; i < bulletNumber; i++)
        {
            // G�n�rer une direction al�atoire dans un c�ne
            Vector3 spreadDirection = Random.insideUnitCircle.normalized * Random.Range(0f, spreadAngle);
            Vector3 direction = new Vector3(Random.Range(-spreadAngle, spreadAngle) * Mathf.Cos(Random.Range(0, 2 * Mathf.PI)), Random.Range(-spreadAngle, spreadAngle) * Mathf.Sin(Random.Range(0, 2 * Mathf.PI)), ralspread);

            // Lancer un raycast dans la direction d�finie
            RaycastHit hit;
            if (Physics.Raycast(muzzlePoint.position, direction, out hit))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Missed!");
            }

            // Affichage visuel du raycast
            Debug.DrawRay(muzzlePoint.position, direction * 100f, Color.red, 3f);
        }
    }
}
