using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;


// Authors: Marc Federspiel, Thomas Kirchhofer
public class NetworkManagerTemp : MonoBehaviour
{
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;
    [SerializeField] private TextMeshProUGUI _readyText;

    void Start()
    {
        SwitchToConnectionChoice();

        _hostBtn.onClick.AddListener(OnStartHost);
        _clientBtn.onClick.AddListener(OnStartClient);
    }

    public async void OnStartHost()
    {
        try
        {
            SwitchToReadyUp();
            _readyText.text = "Initializing host...";
            Debug.Log("Starting Host...");

            if (LobbyManager.Instance == null)
            {
                Debug.LogError("LobbyManager.instance is null");
                _readyText.text = "Failed to start host: LobbyManager not initialized.";
                SwitchToConnectionChoice();
                return;
            }

            await LobbyManager.Instance.Init(NetworkManager.Singleton.LocalClientId);

            var allocation = await RelayService.Instance.CreateAllocationAsync(GameManager.MaxPlayers);
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            await LobbyManager.Instance.UpdateLobbyWithRelayCode(relayJoinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("CharacterScene", LoadSceneMode.Single);
            _readyText.text = "Host started. Loading CharacterScene...";
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start host: " + e);
            _readyText.text = "Failed to start host.";
            SwitchToConnectionChoice();
        }
    }

    public async void OnStartClient()
    {
        if (_clientBtn == null) return;

        try
        {
            SwitchToReadyUp();
            _readyText.text = "Initializing client...";
            Debug.Log("Starting Client...");

            if (LobbyManager.Instance == null)
            {
                Debug.LogError("LobbyManager.instance is null");
                _readyText.text = "Failed to start client: LobbyManager not initialized.";
                SwitchToConnectionChoice();
                return;
            }

            await LobbyManager.Instance.Init(NetworkManager.Singleton.LocalClientId);

            string relayJoinCode = await LobbyManager.Instance.GetRelayJoinCode();

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.StartClient();
            _readyText.text = "Client started. Connecting to host...";
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start client: " + e);
            _readyText.text = "Failed to start client.";
            SwitchToConnectionChoice();
        }
    }

    private void OnServerStarted()
    {
        Debug.Log("Server started successfully.");
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected successfully with ID: {clientId}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.LogError($"Client disconnected with ID: {clientId}");
    }

    private void SwitchToReadyUp()
    {
        _hostBtn.gameObject.SetActive(false);
        _clientBtn?.gameObject.SetActive(false);
    }

    private void SwitchToConnectionChoice()
    {
        _hostBtn.gameObject.SetActive(true);
        _clientBtn.gameObject.SetActive(true);
    }
}