using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreTableScreen : NetworkBehaviour
{
    public TextMeshProUGUI[] scoreTexts;
    private int spawnedClients = 0;

    private void Awake()
    {

    }

    void Start()
    {
    }

    public override void OnNetworkSpawn()
    {

        base.OnNetworkSpawn();

        NetworkManager.SceneManager.OnSceneEvent += onSceneEvent;
    }

    private void onSceneEvent(SceneEvent e)
    {

        if (e.SceneEventType == SceneEventType.LoadEventCompleted)
        {

            if (IsHost)
            {

                countSpawnedClients();
            }
            else
            {

                spawnClientServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void spawnClientServerRpc()
    {

        countSpawnedClients();
    }

    private void countSpawnedClients()
    {

        if (!IsHost) return;

        spawnedClients++;

        if (spawnedClients == GameManager.instance.getPlayerCount())
        {

            loadScoreList();
        }

    }

    public void startNextRound()
    {

        MapLoader.LoadRandomSceneFromFolder();
    }

    private void loadScoreList()
    {

        int[] connectedPlayerIDs = new int[GameManager.MaxPlayers];
        List<PlayerNetwork> players = GameManager.instance.getConnectedPlayers();
        int index = 0;
        foreach (PlayerNetwork player in players)
        {

            int clientID = (int)player.GetComponent<IDHolder>().getClientID();
            if (GameManager.instance.isClientStillConnected(clientID))
            {

                connectedPlayerIDs[index++] = clientID;
            }
        }

        int[] scoreTable = PointManager.instance.getScoreTable();
        loadScoreListClientRpc(scoreTable, connectedPlayerIDs);
    }

    [ClientRpc]
    private void loadScoreListClientRpc(int[] scoreTable, int[] connectedPlayerIDs)
    {


        int textIndex = 0;
        for (int i = 0; i < GameManager.MaxPlayers; i++)
        {

            if (!connectedPlayerIDs.Contains(i)) scoreTexts[textIndex].text = "";

            string text = "Player " + i + " :\t" + scoreTable[i];
            scoreTexts[textIndex].text = text;
            textIndex++;
        }
    }
}
