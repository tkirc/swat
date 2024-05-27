using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Authors: Thomas Kirchhofer
public class LobbyCharacter : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyText;

    private void Start()
    {
        GameManager.instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChanged;
        GameManager.instance.OnReadyChanged += OnReadyChanged;
        UpdatePlayer();
    }

    private void OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (GameManager.instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = GameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyText.SetActive(GameManager.instance.IsPlayerReady(playerData.clientId));
        }
        else {
            Hide();
        }
    }

    private void Show() { 
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
