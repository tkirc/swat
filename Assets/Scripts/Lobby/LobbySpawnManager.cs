using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Federspiel
public class LobbySpawnManager : NetworkBehaviour
{

    public static LobbySpawnManager instance = null;
    [SerializeField] private GameObject[] spawners;

    void Awake()
    {

        instance = this;

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        for (ulong i = 0; i < (ulong)GameManager.MaxPlayers; i++)
        {

            spawners[i].GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        if (!IsHost) return;

        StartCoroutine("spawnHost", 3.0f);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        onDespawnClientRpc();
    }

    [ClientRpc]
    private void onDespawnClientRpc()
    {

        instance = null;
    }

    void Update()
    {

        if (!IsHost) return;

        List<PlayerNetwork> players = GameManager.instance.getConnectedPlayers();
        foreach (PlayerNetwork player in players)
        {

            Quaternion rot = Quaternion.LookRotation(
               Camera.main.transform.position - player.transform.position,
               Vector3.up
                );
            rot.x = 0;
            rot.z = 0;
            player.transform.rotation = rot;
        }
    }

    private void spawnHost()
    {

        spawnPlayer(GameManager.instance.getConnectedPlayers()[0]);
    }

    public void spawnPlayer(PlayerNetwork player)
    {

        player.disableRenderer();
        findFreeSlot(player);

        player.enableRenderer();
    }

    private void findFreeSlot(PlayerNetwork player)
    {

        bool accepted = false;

        int i = 0;
        do
        {

            accepted = spawners[i++].GetComponent<SpawnPoint>().acceptPlayer(player);
        } while (!accepted);

        player.GetComponent<PlayerMovement>().disableLookRotation();
        player.GetComponent<PlayerInput>().disableBattleControls();

        Quaternion rot = Quaternion.LookRotation(
           Camera.main.transform.position - player.transform.position,
           Vector3.up
            );
        rot.x = 0;
        rot.z = 0;
        player.transform.rotation = rot;
    }

    public void readyPlayer(ulong clientID)
    {

        readyPlayerClientRpc(clientID);
    }

    [ClientRpc]
    public void readyPlayerClientRpc(ulong clientID)
    {

        spawners[(int)clientID].GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    public void unreadyPlayer(ulong clientID)
    {

        unreadyPlayerClientRpc(clientID);
    }

    [ClientRpc]
    public void unreadyPlayerClientRpc(ulong clientID)
    {

        spawners[(int)clientID].GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    public void playerLeft(ulong clientID)
    {

        spawners[(int)clientID].GetComponentInChildren<MeshRenderer>().enabled = false;
        spawners[(int)clientID].GetComponent<SpawnPoint>().setOccupied(false);
    }
}
