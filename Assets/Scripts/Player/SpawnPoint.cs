using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Fedespiel
public class SpawnPoint : NetworkBehaviour
{

    [SerializeField]
    private NetworkVariable<bool> occupied =
                            new NetworkVariable<bool>(
                                false,
                                NetworkVariableReadPermission.Everyone,
                                NetworkVariableWritePermission.Server
                            );

    public bool acceptPlayer(PlayerNetwork player)
    {

        if (occupied.Value) return false;

        player.transform.position = transform.position;

        occupied.Value = true;

        return true;
    }

    public bool isOccupied()
    {

        return occupied.Value;
    }

    public void setOccupied(bool b)
    {

        occupied.Value = b;
    }
}
