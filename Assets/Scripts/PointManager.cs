using TMPro;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Health;
using System.Collections.Generic;

// Authors: Marc Fedespiel
public class PointManager : NetworkBehaviour
{
    public static PointManager instance = null;

    // Class to Save Points of one Player.
    public int maxPlayers = 4;

    [SerializeField]
    private NetworkVariable<ScoreTable> scoreTable =
        new NetworkVariable<ScoreTable>(
            new ScoreTable(GameManager.MaxPlayers),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    private bool subscribedToCallbacks = false;

    void Awake()
    {

        maxPlayers = GameManager.MaxPlayers;
        if (UnityEngine.Object.ReferenceEquals(instance, null))
        {
            instance = this;
            GetComponent<NetworkObject>().Spawn();
            DontDestroyOnLoad(gameObject); // Make sure That GameObject won't be Destroyed by loading in Other Scenes.
            Initialize();
        }
        else
        {
            Destroy(gameObject); // If instance already exits, destroy the gameobject
        }
    }

    void Start()
    {

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log("OnNetworkSpawn");
        if (!IsHost) return;
        if (subscribedToCallbacks) return;

        List<PlayerNetwork> players = GameManager.instance.getConnectedPlayers();
        foreach (PlayerNetwork p in players)
        {

            Health health = p.GetComponent<Health>();
            health.registerOnDeathCallback(AddOnePoint);
        }

        subscribedToCallbacks = true;
    }

    void OnDestroy()
    {

        Debug.Log("Destroyed");
    }

    public void Initialize()
    {
    }

    /*
    public static PointManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("PointManager");
                instance = obj.AddComponent<PointManager>();
                instance.Initialize();
                DontDestroyOnLoad(obj); // Make sure That GameObject won't be Destroyed by loading in Other Scenes.
            }
            return instance;
        }
    }
*/
    public void AddOnePoint(ulong playerIndex)
    {

        AddPoints((int)playerIndex, 1);
    }

    public void AddPoints(int playerIndex, int amount)
    {

        if (GameManager.instance.isClientStillConnected(playerIndex))
        {

            scoreTable?.Value.addPoints(playerIndex, amount);
        }
    }

    public void ReducePoints(int playerIndex, int amount)
    {

        if (GameManager.instance.isClientStillConnected(playerIndex))
        {

            scoreTable.Value.reducePoints(playerIndex, amount);
        }
    }

    public void SetPoints(int playerIndex, int amount)
    {

        if (GameManager.instance.isClientStillConnected(playerIndex))
        {

            scoreTable.Value.setPoints(playerIndex, amount);
        }
    }

    public int getPoints(int playerIndex)
    {

        if (GameManager.instance.isClientStillConnected(playerIndex))
        {

            return scoreTable.Value.getPointsOfPlayer(playerIndex);
        }

        return -1;

        // Never throw an exception
        // if one player disconnets the game might crash for everyone
        //throw new System.Exception("Points: Index Number is outside of Range");
    }

    public int[] getScoreTable()
    {

        return scoreTable.Value.asArray();
    }

}