using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Fedespiel
[System.Serializable]
public struct ScoreTable : INetworkSerializable
{

    [SerializeField] private int[] scores;

    public ScoreTable(int playerCount)
    {

        scores = new int[playerCount];
    }

    public void addPoints(int index, int amount)
    {

        scores[index] += amount;
    }

    public void reducePoints(int index, int amount)
    {

        scores[index] -= amount;
    }

    public void setPoints(int index, int amount)
    {

        scores[index] = amount;
    }

    public int getPointsOfPlayer(int index)
    {

        return scores[index];
    }

    public int[] asArray()
    {

        return scores;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {

        for (int i = 0; i < scores.Length; i++)
        {

            serializer.SerializeValue<int>(ref scores[i]);
        }

    }
}
