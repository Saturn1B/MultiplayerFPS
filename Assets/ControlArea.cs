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

    // Définis le temps entre chaque incrément de point
    public float captureInterval = 1f;

    void Start()
    {
        isNeutral = true;
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
        if (redCap)
        {
            redPoint += 1; // Ajoute un point par seconde pendant la capture
        }
        else if (blueCap)
        {
            bluePoint += 1; // Ajoute un point par seconde pendant la capture
        }
        else if (isNeutral)
        {
            // La zone est neutre, tu peux ajuster le comportement ici si nécessaire.
        }

        // Réinitialise les indicateurs de capture après avoir pris en compte la capture
        redCap = false;
        blueCap = false;
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
