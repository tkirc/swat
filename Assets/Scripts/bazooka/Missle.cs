using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Federspiel
public class Missle : Projectile
{


    [SerializeField] private GameObject blastRadiusPrefab;
    [SerializeField] private float force;

    void Start()
    {

        if (!IsHost) return;

        GetComponent<Rigidbody>().AddForce(transform.forward * force);
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        if (!IsHost) return;

        int layer = 1 << collision.gameObject.layer;

        if (layer == LayerMask.GetMask("Wall") ||
            layer == LayerMask.GetMask("Ground") ||
            layer == LayerMask.GetMask("Enemy") ||
            layer == LayerMask.GetMask("Player"))
        {


            explode(
                collision.contacts[0].point,
                GetComponent<Rigidbody>().velocity.normalized,
                collision.contacts[0].normal
            );

        }
    }

    public void explode(Vector3 position, Vector3 direction, Vector3 normal)
    {

        GameObject g = Instantiate(blastRadiusPrefab, transform.position, transform.rotation);
        g.GetComponent<IDHolder>().setClientID(GetComponent<IDHolder>().getClientID());
        g.layer = LayerMask.NameToLayer("Damaging");
        g.GetComponent<NetworkObject>().Spawn(true);

        Destroy(gameObject);
    }


}
