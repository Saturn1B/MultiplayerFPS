using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string[] sceneNames; // Tableau de noms de scènes

    void Start()
    {
        // Accéder aux noms de scènes dans le tableau
        foreach (string sceneName in sceneNames)
        {
            Debug.Log("Nom de la scène : " + sceneName);
        }
    }
}
