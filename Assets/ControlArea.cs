using System.Collections;
using UnityEngine;

public class ControlArea : MonoBehaviour
{
    [SerializeField] public bool redCap;
    [SerializeField] public bool blueCap;
    [SerializeField] public bool isNeutral;

    int redPoint;
    int bluePoint;

    // temps entre chaque incrément de point
    public float captureInterval = 1f;

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
            timeSinceCaptureStart = 0f;
        }
        else if (other.CompareTag("Blue"))
        {
            blueCap = true;
            timeSinceCaptureStart = 0f;
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
            timeSinceCaptureStart = 0f;
        }
    }

    public void Capture()
    {
        timeSinceCaptureStart += Time.deltaTime;

        if (timeSinceCaptureStart >= captureDelay)
        {
            timeSinceLastCapture += Time.deltaTime;

            if (timeSinceLastCapture >= captureInterval)
            {
                if (redCap && !blueCap)
                {
                    redPoint += 1;
                }
                else if (blueCap && !redCap)
                {
                    bluePoint += 1;
                }

                timeSinceLastCapture = 0f; // Réinitialise le temps depuis la dernière incrémentation des points
            }

            if (isNeutral)
            {
                Debug.Log("Neutre");
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
