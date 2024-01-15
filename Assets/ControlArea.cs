using System.Collections;
using UnityEngine;

public class ControlArea : MonoBehaviour
{
    [SerializeField] public bool redCap;
    [SerializeField] public bool blueCap;
    [SerializeField] public bool isNeutral;

    int redPoint;
    int bluePoint;

    // Définis le temps entre chaque incrément de point
    public float captureInterval = 1f;

    // Délai avant que l'équipe commence à gagner des points
    public float captureDelay = 3f;

    // Temps depuis le début du délai
    private float timeSinceCaptureStart;

    // Temps depuis la dernière incrémentation des points
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
            timeSinceCaptureStart = 0f; // Réinitialise le temps depuis le début du délai
        }
        else if (other.CompareTag("Blue"))
        {
            blueCap = true;
            timeSinceCaptureStart = 0f; // Réinitialise le temps depuis le début du délai
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Red"))
        {
            redCap = false;
            timeSinceCaptureStart = 0f; // Réinitialise le temps depuis le début du délai si l'équipe quitte la zone
        }
        else if (other.CompareTag("Blue"))
        {
            blueCap = false;
            timeSinceCaptureStart = 0f; // Réinitialise le temps depuis le début du délai si l'équipe quitte la zone
        }
    }

    public void Capture()
    {
        timeSinceCaptureStart += Time.deltaTime;

        // Ajoute un délai de 3 secondes avant de commencer à gagner des points
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

                timeSinceLastCapture = 0f; // Réinitialise le temps depuis la dernière incrémentation des points
            }

            if (isNeutral)
            {
                Debug.Log("Neutre"); // La zone est neutre, tu peux ajuster le comportement ici si nécessaire.
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
