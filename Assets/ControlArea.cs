using System.Collections;
using UnityEngine;

public class ControlArea : MonoBehaviour
{
    [SerializeField]
    public bool redCap;
    [SerializeField]
    public bool blueCap;
    [SerializeField]
    public bool isNeutral;

    int redPoint;
    int bluePoint;

    // D�finis le temps entre chaque incr�ment de point
    public float captureInterval = 1f;

    // Temps depuis la derni�re incr�mentation des points
    private float timeSinceLastCapture;

    void Start()
    {
        isNeutral = true;
        timeSinceLastCapture = 0f;
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
        }
        else if (other.CompareTag("Blue"))
        {
            blueCap = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Red"))
        {
            redCap = false;
        }
        else if (other.CompareTag("Blue"))
        {
            blueCap = false;
        }
    }

    public void Capture()
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

    IEnumerator UpdatePoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(captureInterval);

            Debug.Log("Red Team Points: " + redPoint + " | Blue Team Points: " + bluePoint);
        }
    }
}
