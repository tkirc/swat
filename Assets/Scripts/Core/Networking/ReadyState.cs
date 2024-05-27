using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Federspiel
[System.Serializable]
public struct ReadyState : INetworkSerializable
{

    [SerializeField] private bool[] ready;

    public ReadyState(int playerCount)
    {

        ready = new bool[playerCount];
    }

    public void readyPlayer(int i)
    {

        ready[i] = true;
    }

    public void unreadyPlayer(int i)
    {

        ready[i] = false;
    }

    public readonly bool IsReady(int i)
    {
        return ready[i];
    }

    public bool allReady(int count)
    {

        int readyPlayers = 0;
        foreach (bool b in ready)
        {

            if (b)
            {

                readyPlayers++;
            }
        }

        return readyPlayers == count;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        for (int i = 0; i < ready.Length; i++)
        {

            serializer.SerializeValue<bool>(ref ready[i]);
        }
    }

    public void log()
    {

        for (int i = 0; i < ready.Length; i++)
        {
            Debug.Log(string.Format("ready[{0}] = {1}", i, ready[i]));
        }
    }
}
