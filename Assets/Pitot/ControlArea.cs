using System.Collections;
using UnityEngine;

public class ControlArea : MonoBehaviour
{
    [SerializeField] public bool redCap;
    [SerializeField] public bool blueCap;
    [SerializeField] public bool isNeutral;

    int redPoint;
    int bluePoint;

    // temps entre chaque incr�ment de point
    public float captureInterval = 1f;

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
            timeSinceCaptureStart = 0f; // R�initialise le temps depuis le d�but du d�lai si l'�quipe quitte la zone
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

                timeSinceLastCapture = 0f; // R�initialise le temps depuis la derni�re incr�mentation des points
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
