using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Fedespiel
public class IDHolder : NetworkBehaviour
{

    [SerializeField]
    private NetworkVariable<ulong> clientID =
                            new NetworkVariable<ulong>(
                                ulong.MaxValue,
                                NetworkVariableReadPermission.Everyone,
                                NetworkVariableWritePermission.Owner
                            );

    public ulong getClientID()
    {

        return clientID.Value;
    }

    public void setClientID(ulong clientID)
    {

        this.clientID.Value = clientID;
    }

    public void fetchLocalClientID()
    {

        fetchLocalClientIClientRpc();
    }

    [ClientRpc]
    public void fetchLocalClientIClientRpc()
    {

        if (!IsOwner) return;

        clientID.Value = NetworkManager.Singleton.LocalClientId;
    }
}
