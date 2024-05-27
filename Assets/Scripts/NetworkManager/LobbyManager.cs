using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = System.Random;

// Authors: Marc Federspiel, Thomas Kirchhofer
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance = null;
    private Lobby _hostLobby = null;
    private float _heartBeatTimer = 60.0f;

    void Awake()
    {
        Debug.Log("LobbyManager awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void Update()
    {
        SendHeartBeat();
    }

    private async void SendHeartBeat()
    {
        if (_hostLobby == null) return;

        _heartBeatTimer -= Time.deltaTime;

        if (_heartBeatTimer < 0.0f)
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
                _heartBeatTimer = 60.0f;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Error sending heartbeat: " + e);

                _heartBeatTimer = 120.0f;
            }
        }
    }

    public async Task Init(ulong clientId)
    {
        Debug.Log("Initializing LobbyManager with clientID: " + clientId);
        try
        {
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += OnSignIn;
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (clientId == 0)
            {
                await CreateLobby(clientId);
            }
            else
            {
                await JoinLobby(clientId);
            }
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogError("Failed to initialize services: " + e);
        }
        catch (AuthenticationException e)
        {
            Debug.LogError("Failed to authenticate: " + e);
        }
    }

    private void OnSignIn()
    {
        Debug.Log("Logged in: " + AuthenticationService.Instance.PlayerId);
        if (LobbyInfo.Instance != null)
        {
            LobbyInfo.Instance.SetAdditionalInfo("Logged in: " + AuthenticationService.Instance.PlayerId);
        }
    }

    public async Task CreateLobby(ulong clientId)
    {
        Debug.Log("Creating lobby...");
        try
        {
            var random = new Random();
            var lobbyName = "Lobby" + random.Next(1000);
            _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameManager.MaxPlayers);
            if (_hostLobby != null)
            {
                Debug.Log("Lobby created with ID: " + _hostLobby.Id);
                Debug.Log("Created Lobby: " + lobbyName);
                if (LobbyInfo.Instance != null)
                {
                    LobbyInfo.Instance.SetAdditionalInfo("LobbyId: " + _hostLobby.Id);
                    LobbyInfo.Instance.SetLobbyInfo("LobbyName: " + lobbyName);
                }
            }
            else
            {
                Debug.LogWarning("No lobby created");
                if (LobbyInfo.Instance == null)
                {
                    Debug.LogError("LobbyInfo is null");
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("CreateLobby: " + e);
        }

        await ListLobbies(clientId);
    }

    public async Task UpdateLobbyWithRelayCode(string relayJoinCode)
    {
        if (_hostLobby == null)
        {
            Debug.LogError("hostLobby is null. Cannot update with relay join code.");
            return;
        }

        var options = new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) }
            }
        };

        await LobbyService.Instance.UpdateLobbyAsync(_hostLobby.Id, options);
        Debug.Log("Updated lobby with relay join code: " + relayJoinCode);
    }

    public async Task<string> GetRelayJoinCode()
    {
        var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
        var lobby = queryResponse.Results[0];
        if (lobby.Data.ContainsKey("RelayJoinCode"))
        {
            return lobby.Data["RelayJoinCode"].Value;
        }

        Debug.LogError("No RelayJoinCode found in lobby data.");
        return null;
    }

    public async Task ListLobbies(ulong clientId)
    {
        Debug.Log("Listing lobbies...");
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log(clientId + " Lobbies found: " + response.Results.Count);
            foreach (Lobby l in response.Results)
            {
                Debug.Log(clientId + " " + l.Name + ": " + l.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("ListLobbies: " + e);
        }
    }

    public async Task JoinLobby(ulong clientId)
    {
        await ListLobbies(clientId);
        Debug.Log("Joining lobby...");
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            string s = "Lobbies available:\n";
            foreach (Lobby l in response.Results)
            {
                s += "Name: " + l.Name + "\tID: " + l.Id + "\n";
            }
            if (LobbyInfo.Instance != null)
            {
                LobbyInfo.Instance.SetLobbyInfo(s);
            }

            if (response.Results.Count > 0)
            {
                try
                {
                    Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(response.Results[0].Id);
                    if (lobby != null)
                    {
                        Debug.Log("Joined Lobby: " + lobby.Name);
                    }
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogError("JoinLobby: " + e);
                }
            }
        }
        catch (LobbyServiceException e)
        {
            if (LobbyInfo.Instance != null)
            {
                LobbyInfo.Instance.SetAdditionalInfo(e.Message);
            }
        }
    }
}