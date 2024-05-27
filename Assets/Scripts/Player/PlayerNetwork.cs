using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

// Authors: Marc Federspiel
public class PlayerNetwork : NetworkBehaviour
{

    public static PlayerNetwork localPlayer;


    [Header("Debug Info")]
    [SerializeField] private bool isHost;
    [SerializeField] private bool isOwner;
    [SerializeField] private bool isClient;


    public delegate void UseCallback();
    private UseCallback onUse;

    void Awake()
    {

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }


    void Start()
    {

        if (IsOwner)
        {

            localPlayer = this;
            GetComponent<IDHolder>().setClientID(NetworkManager.Singleton.LocalClientId);
        }
        else
        {

            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.materials[1].SetColor("_color", Color.red);
        }

        isHost = IsHost;
        isOwner = IsOwner;
        isClient = IsClient;
    }

    public void enableRenderer()
    {

        enableRendererClientRpc();
    }

    [ClientRpc]
    public void enableRendererClientRpc()
    {

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = true;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m in mrs)
        {

            m.enabled = true;
            m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    public void disableRenderer()
    {

        disableRendererClientRpc();
    }

    [ClientRpc]
    public void disableRendererClientRpc()
    {

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m in mrs)
        {

            m.enabled = false;
            m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }





}
