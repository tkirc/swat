using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

// Authors: Marc Fedespiel
public class GameMonitor : NetworkBehaviour
{
    public static GameMonitor instance = null;
    [SerializeField] public int winningConditionScore = 10;
    public static int sceneIndex;
    public GameManager gameManager;

    private void Awake()
    {

        instance = this;
        gameManager = GameManager.instance;
    }

    public void Start()
    {

        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        DontDestroyOnLoad(gameObject);
    }


    public void roundConcluded()
    {

        int winningPlayer = getWinningPlayer();
        if (winningPlayer != -1)
        {
            Debug.Log("Winner: Player" + winningPlayer);
            loadWinningScene(winningPlayer);
        }
        else
        {
            NetworkManager.Singleton.SceneManager.LoadScene("scoretable", LoadSceneMode.Single);
        }
    }


    private void loadWinningScene(int winningPlayerIndex)
    {
        PlayerPrefs.SetInt("WinningPlayer", winningPlayerIndex);
        NetworkManager.Singleton.SceneManager.LoadScene("WinningScene", LoadSceneMode.Single);
    }

    private int getWinningPlayer()
    {

        int playerWithHighestScore = -1;
        int highestScore = 0;

        List<PlayerNetwork> players = GameManager.instance.getConnectedPlayers();

        if (players.Count == 1)
        {

            return (int)players[0].GetComponent<IDHolder>().getClientID();
        }

        foreach (PlayerNetwork player in players)
        {

            ulong clientID = player.GetComponent<IDHolder>().getClientID();
            int score = PointManager.instance.getPoints((int)clientID);
            if (score >= winningConditionScore && score > highestScore)
            {

                playerWithHighestScore = (int)clientID;
                highestScore = score;
            }
        }

        return playerWithHighestScore;
    }
}
