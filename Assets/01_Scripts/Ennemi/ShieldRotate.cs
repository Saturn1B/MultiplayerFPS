using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotate : MonoBehaviour
{
    public Transform[] objectsToRotateAround;
    public float rotationSpeed = 3.0f;

    private int currentIndex = 0;

    void Update()
    {
        Transform target = objectsToRotateAround[currentIndex];
        Vector3 targetPosition = target.position;

        // Faites tourner l'objet actuel autour du prochain objet
        transform.RotateAround(targetPosition, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
