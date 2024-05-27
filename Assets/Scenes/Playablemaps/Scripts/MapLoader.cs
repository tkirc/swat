using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoader : NetworkBehaviour
{
    public static string sceneFolderPath = "Assets/Scenes/Playablemaps";
    private static int[] sceneIndeces = { 7, 8, 9, 10 };

    public static void LoadRandomSceneFromFolder()
    {

        // Get all scenes from a folder
        //string[] sceneFiles = Directory.GetFiles(sceneFolderPath, "*.unity");
        string[] sceneFiles = new string[sceneIndeces.Length];
        for (int i = 0; i < sceneIndeces.Length; i++)
        {

            sceneFiles[i] = SceneUtility.GetScenePathByBuildIndex(sceneIndeces[i]);
        }

        if (sceneFiles.Length > 0)
        {
            string randomScenePath = sceneFiles[Random.Range(0, sceneFiles.Length)];

            NetworkManager.Singleton.SceneManager.LoadScene(Path.GetFileNameWithoutExtension(randomScenePath), LoadSceneMode.Single);
            //SceneManager.LoadScene(Path.GetFileNameWithoutExtension(randomScenePath));
        }
        else
        {
            Debug.LogWarning("Keine Szenen im angegebenen Ordner gefunden: " + sceneFolderPath);
        }
    }

    public static void loadMap(string mapnumber)
    {
        string scenePath = Path.Combine(sceneFolderPath, "Map_" + mapnumber + ".unity");

        if (File.Exists(scenePath))
        {
            NetworkManager.Singleton.SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scenePath), LoadSceneMode.Single);

        }
        else
        {
            Debug.LogWarning("Szene '" + "Map_" + mapnumber + "' nicht gefunden im Ordner '" + sceneFolderPath + "'.");
        }

    }
}