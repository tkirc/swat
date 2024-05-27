using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenuUI : MonoBehaviour
{

    [SerializeField] private Canvas startMenuCanvas;
    [SerializeField] private Canvas controlsMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {

        controlsMenuCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void onStartButtonPressed()
    {

        startMenuCanvas.enabled = false;
        SceneManager.LoadScene(2);
    }

    public void onControlsButtonPressed()
    {

        startMenuCanvas.enabled = false;
        controlsMenuCanvas.enabled = true;
    }

    public void onControlsBackButtonPressed()
    {

        controlsMenuCanvas.enabled = false;
        startMenuCanvas.enabled = true;
    }

    public void onQuitPressed()
    {

        Application.Quit();
    }
}
