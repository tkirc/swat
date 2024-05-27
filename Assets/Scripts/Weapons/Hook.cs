using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

// Authors: Marc Fedespiel
public class Hook : Projectile
{

    private HookShot hookShot;
    public delegate void DropCallback();
    public DropCallback onHit;

    private bool updateLineRenderer;

    [SerializeField] private float force = 1000;


    void Start()
    {

        updateLineRenderer = false;
        hookShot = GetComponentInParent<HookShot>();
    }

    public void registerDropCallback(DropCallback callback)
    {

        onHit = callback;
    }

    public void shoot()
    {

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForce(transform.up * force);
        rb.useGravity = false;

        updateLineRenderer = true;
    }


    public void setLayer(ulong shooterID)
    {

        setLayerClientRpc(shooterID);
    }

    [ClientRpc]
    public void setLayerClientRpc(ulong shooterID)
    {

        if (shooterID == NetworkManager.Singleton.LocalClientId) return;

        gameObject.layer = LayerMask.NameToLayer("Damaging");
    }

    void FixedUpdate()
    {
        if (!IsHost) return;
        if (!updateLineRenderer) return;

        hookShot.updateLineRenderer();
    }

    void OnCollisionEnter(Collision collision)
    {


        int layer = 1 << collision.gameObject.layer;
        Rigidbody rb = GetComponent<Rigidbody>();
        ulong clientID = GetComponent<IDHolder>().getClientID();

        if (layer == LayerMask.GetMask("Player"))
        {

            ulong hitClientID = collision.gameObject.GetComponent<IDHolder>().getClientID();

            if (clientID == hitClientID) return;

            Debug.Log("hit from Hook: " + hitClientID);

            rb.velocity = Vector3.zero;
            PlayerNetwork player = GameManager.instance.getPlayerWithID(clientID);
            player.GetComponent<PlayerMovement>().enableLookRotation();
            player.GetComponent<PlayerInput>().enableBattleControls();

            player.transform.position = collision.transform.position;

            updateLineRenderer = false;
            onHitServerRpc();
        }
        else
        {

            rb.velocity = Vector3.zero;
            PlayerNetwork player = GameManager.instance.getPlayerWithID(clientID);
            player.GetComponent<PlayerMovement>().enableLookRotation();
            player.GetComponent<PlayerInput>().enableBattleControls();

            updateLineRenderer = false;
            onHitServerRpc();
        }
    }

    [ServerRpc]
    void onHitServerRpc()
    {


        onHit?.Invoke();
    }
}
