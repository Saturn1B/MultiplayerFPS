using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string[] sceneNames; // Tableau de noms de sc�nes

    void Start()
    {
        // Acc�der aux noms de sc�nes dans le tableau
        foreach (string sceneName in sceneNames)
        {
            Debug.Log("Nom de la sc�ne : " + sceneName);
        }
    }
}
