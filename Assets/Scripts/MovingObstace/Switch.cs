using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Authors: Marc Federspiel
public class Switch : NetworkBehaviour
{

    [SerializeField] private Material onMeterial;
    [SerializeField] private Material offMeterial;

    [Header("References")]
    [Header("All Buttons Affected")]
    [SerializeField] private GameObject[] buttons;
    [Header("Other Switches Affected")]
    [SerializeField] private Switch[] swicthes;
    [Header("Object Affected")]
    [SerializeField] private OscelatingMovement oscelating;

    [SerializeField]
    private NetworkVariable<bool> on =
            new NetworkVariable<bool>(
                false,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    void Start()
    {

        onChange(true, false);
        on.OnValueChanged += onChange;
    }

    private void onChange(bool oldValue, bool newValue)
    {

        Material activeMaterial = newValue ? onMeterial : offMeterial;

        foreach (GameObject b in buttons)
        {

            MeshRenderer mr = b.GetComponent<MeshRenderer>();
            mr.material = activeMaterial;
        }

        if (IsHost)
        {

            if (newValue)
            {

                oscelating.activate();
            }
            else
            {

                oscelating.deactivate();
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {


        if ((1 << collider.gameObject.layer) == LayerMask.GetMask("Player"))
        {

            if (!collider.gameObject.GetComponent<PlayerNetwork>().IsOwner) return;

            startListeningToClient();
        }
    }

    public void OnTriggerExit(Collider collider)
    {


        if ((1 << collider.gameObject.layer) == LayerMask.GetMask("Player"))
        {

            if (!collider.gameObject.GetComponent<PlayerNetwork>().IsOwner) return;

            stopListeningToClient();
        }
    }



    private void startListeningToClient()
    {


        PlayerNetwork p = PlayerNetwork.localPlayer;
        p.GetComponent<PlayerInput>().registerOnUseCallback(buttonPressed);
    }

    private void stopListeningToClient()
    {

        PlayerNetwork p = PlayerNetwork.localPlayer;
        p.GetComponent<PlayerInput>().unregisterOnUseCallback(buttonPressed);
    }

    public void buttonPressed()
    {

        toggleOnOffServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void toggleOnOffServerRpc(ulong clientID)
    {

        on.Value = !on.Value;

        foreach (Switch s in swicthes)
        {

            s.setState(on.Value);
        }
    }

    public void setState(bool on)
    {

        this.on.Value = on;
    }

}
