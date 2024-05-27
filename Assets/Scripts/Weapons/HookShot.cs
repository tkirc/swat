using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Fedespiel
public class HookShot : WeaponNetwork
{


    [SerializeField] private GameObject handle;
    [SerializeField] private GameObject hook;
    [SerializeField] private float force = 200;
    private LineRenderer lineRenderer;

    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();

        Vector3[] positions = new Vector3[2];
        positions[0] = transform.position;
        positions[1] = hook.transform.position;
        lineRenderer.SetPositions(positions);
    }

    public override void shoot()
    {

        shootServerRpc(GetComponent<IDHolder>().getClientID());
    }

    [ServerRpc]
    public override void shootServerRpc(ulong clientID)
    {

        if (ammo.Value <= 0) return;

        ammo.Value--;

        hook.GetComponent<IDHolder>().setClientID(clientID);
        hook.transform.SetParent(null);
        hook.GetComponent<CapsuleCollider>().enabled = true;

        Hook hookObject = hook.GetComponent<Hook>();
        hookObject.registerDropCallback(dropOnServer);
        hookObject.setLayer(clientID);
        hookObject.shoot();


        PlayerNetwork player = GameManager.instance.getPlayerWithID(clientID);
        player.GetComponent<PlayerMovement>().disableLookRotation();
        player.GetComponent<PlayerInput>().disableBattleControls();
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void updateLineRenderer()
    {

        updateLineRendererClientRpc();
    }

    [ClientRpc]
    private void updateLineRendererClientRpc()
    {

        Vector3[] positions = new Vector3[2];
        positions[0] = transform.position;
        positions[1] = hook.transform.position;
        lineRenderer.SetPositions(positions);
    }


    public override void drop()
    {

    }

    [ServerRpc]
    public override void dropServerRpc()
    {

        dropOnServer();
    }

    public void dropOnServer()
    {

        transform.SetParent(null);
        PlayerNetwork player = GameManager.instance.getPlayerWithID(GetComponent<IDHolder>().getClientID());
        player.GetComponent<WeaponHolder>().dropWeapon();
        gameObject.AddComponent<Rigidbody>();

        gameObject.AddComponent<Destructor>().setDuration(3.0f);

        dropClientRpc();
    }

    [ClientRpc]
    private void dropClientRpc()
    {

        Destroy(lineRenderer);
        Destroy(hook);
    }


    public override bool isEmpty()
    {

        return false;
    }

}
