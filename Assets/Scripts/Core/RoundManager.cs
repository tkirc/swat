using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : NetworkBehaviour
{

    public static RoundManager instance = null;
    [SerializeField] public int winningConditionScore = 10;

    private List<GameObject> toActivate = new List<GameObject>();

    [SerializeField]
    private int deadPlayers = 0;
    private int spawnedClients = 0;
    void Awake()
    {

        instance = this;
    }



    public override void OnNetworkSpawn()
    {


        NetworkManager.SceneManager.OnSceneEvent += onSceneEvent;

        base.OnNetworkSpawn();

        if (!IsHost) return;

        NetworkManager.Singleton.OnClientDisconnectCallback += acknowledgeDisconnect;

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

            startRound();
        }

    }

    public void acknowledgeSuicide()
    {

        ackgnowledgeDeath();
    }

    public void acknowledgeDisconnect(ulong clientID)
    {

        if (playersAlive() <= 1)
        {

            endRound();
        }
    }

    public void ackgnowledgeDeath(ulong shooterID = ulong.MaxValue)
    {

        deadPlayers++;
        if (playersAlive() <= 1)
        {

            endRound();
        }
    }

    public int playersAlive()
    {

        return GameManager.instance.getPlayerCount() - deadPlayers;
    }

    private void forFirstRound()
    {

        AudioManager.instance.playIntro();

        List<PlayerNetwork> players = GameManager.instance.getConnectedPlayers();
        foreach (PlayerNetwork player in players)
        {

            GameObject g = (GameObject)Instantiate(Resources.Load("WeaponsNetwork/BaseballBat"));
            g.GetComponent<NetworkObject>().Spawn(true);
            player.GetComponent<WeaponHolder>().pickupWeapon(g.GetComponent<Bat>());
            g.GetComponent<Bat>().onPickup(player.gameObject);
        }
    }

    void startRound()
    {

        if (!IsHost) return;

        if (GameManager.instance.isFirstRound())
        {

            forFirstRound();
            GameManager.instance.setFirstRound(false);
        }

        List<PlayerNetwork> players = GameManager.instance.getConnectedPlayers();

        while (UnityEngine.Object.ReferenceEquals(PointManager.instance, null)) { }

        foreach (PlayerNetwork player in players)
        {

            player.GetComponent<PlayerInput>().enableBattleControls();
            player.GetComponent<PlayerMovement>().enableLookRotation();
            player.enableRenderer();
            player.GetComponent<WeaponHolder>().enableWeaponPickup();

            Health h = player.GetComponent<Health>();
            h.resetHealth();
            h.registerOnDeathCallback(ackgnowledgeDeath);
            h.registerOnSuicideCallback(acknowledgeSuicide);

            PlayerSpawner.instance.SpawnPlayer(player);
        }


        foreach (GameObject g in toActivate)
        {

            if (g.TryGetComponent<WeaponSpawnerNetwork>(out WeaponSpawnerNetwork w))
            {

                w.acitvate();
            }
        }
    }


    public void endRound()
    {

        Debug.Log("END ROUND");
        List<PlayerNetwork> players = GameManager.instance.getConnectedPlayers();

        foreach (PlayerNetwork player in players)
        {

            player.GetComponent<PlayerInput>().disableBattleControls();
            player.GetComponent<PlayerMovement>().disableLookRotation();
            WeaponHolder weaponHolder = player.GetComponent<WeaponHolder>();
            weaponHolder.dropWeapon();
            weaponHolder.disableWeaponPickup();
        }

        GameMonitor.instance.roundConcluded();
        endGame();

        //SceneManager.LoadSceneAsync(5);

    }

    public void endGame()
    {

    }



    public void addGameObjectToActivate(GameObject g)
    {

        toActivate.Add(g);
    }



}
