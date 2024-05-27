using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Authors: Marc Federspiel
public class LobbyInfo : MonoBehaviour
{

    public static LobbyInfo Instance = null;
    [SerializeField] private TextMeshProUGUI _lobbyInfo;
    [SerializeField] private TextMeshProUGUI _lobbyAdditionalInfo;

    void Awake()
    {

        Instance = this;
    }
    void Start()
    {

        DontDestroyOnLoad(gameObject);

    }

    public void SetLobbyInfo(string text)
    {

        if (text == null)
        {

            text = "none";
        }

        _lobbyInfo.text = "Lobby: " + text;
    }

    public void SetAdditionalInfo(string text)
    {

        _lobbyAdditionalInfo.text = text;
    }
}
