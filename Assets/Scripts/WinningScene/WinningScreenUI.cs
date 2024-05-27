using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WinningScreenUI : MonoBehaviour
{
    [SerializeField] private Canvas winningScreenCanvas;
    [SerializeField] private TextMeshProUGUI winningPlayerText;
    [SerializeField] private Button ReturnToMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        setWinningPlayerText();
        winningScreenCanvas.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void setWinningPlayerText()
    {
        int playerScore = PlayerPrefs.GetInt("WinningPlayer");
        winningPlayerText.text = "Player " + playerScore.ToString() + " won the game!";
    }

    public void onReturnToMenuButtonPressend()
    {

        winningScreenCanvas.enabled = false;
        SceneManager.LoadScene("StartMenuScene");
    }
}
