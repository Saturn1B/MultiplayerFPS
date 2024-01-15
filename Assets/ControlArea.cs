using System.Collections;
using UnityEngine;

public class ControlArea : MonoBehaviour
{
    [SerializeField] public bool redCap;
    [SerializeField] public bool blueCap;
    [SerializeField] public bool isNeutral;

    int redPoint;
    int bluePoint;

    // D�finis le temps entre chaque incr�ment de point
    public float captureInterval = 1f;

    // D�lai avant que l'�quipe commence � gagner des points
    public float captureDelay = 3f;

    // Temps depuis le d�but du d�lai
    private float timeSinceCaptureStart;

    // Temps depuis la derni�re incr�mentation des points
    private float timeSinceLastCapture;

    void Start()
    {
        isNeutral = true;
        timeSinceLastCapture = 0f;
        timeSinceCaptureStart = 0f;
        StartCoroutine(UpdatePoints());
    }

    void Update()
    {
        Capture();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Red"))
        {
            redCap = true;
            timeSinceCaptureStart = 0f; // R�initialise le temps depuis le d�but du d�lai
        }
        else if (other.CompareTag("Blue"))
        {
            blueCap = true;
            timeSinceCaptureStart = 0f; // R�initialise le temps depuis le d�but du d�lai
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Red"))
        {
            redCap = false;
            timeSinceCaptureStart = 0f; // R�initialise le temps depuis le d�but du d�lai si l'�quipe quitte la zone
        }
        else if (other.CompareTag("Blue"))
        {
            blueCap = false;
            timeSinceCaptureStart = 0f; // R�initialise le temps depuis le d�but du d�lai si l'�quipe quitte la zone
        }
    }

    public void Capture()
    {
        timeSinceCaptureStart += Time.deltaTime;

        // Ajoute un d�lai de 3 secondes avant de commencer � gagner des points
        if (timeSinceCaptureStart >= captureDelay)
        {
            timeSinceLastCapture += Time.deltaTime;

            if (timeSinceLastCapture >= captureInterval)
            {
                if (redCap && !blueCap)
                {
                    redPoint += 1; // Ajoute un point par seconde pendant la capture
                }
                else if (blueCap && !redCap)
                {
                    bluePoint += 1; // Ajoute un point par seconde pendant la capture
                }

                timeSinceLastCapture = 0f; // R�initialise le temps depuis la derni�re incr�mentation des points
            }

            if (isNeutral)
            {
                Debug.Log("Neutre"); // La zone est neutre, tu peux ajuster le comportement ici si n�cessaire.
            }
        }
    }

    IEnumerator UpdatePoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(captureInterval);

            Debug.Log("Red Team Points: " + redPoint + " | Blue Team Points: " + bluePoint);
        }
    }
}
