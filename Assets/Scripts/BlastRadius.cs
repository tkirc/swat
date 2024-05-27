using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Fedespiel
public class BlastRadius : NetworkBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explostionClip;

    void Start()
    {


        if (!IsHost) return;

        GetComponent<SphereCollider>().radius = radius;

        GameObject g = Instantiate(explosionPrefab, transform.position, transform.rotation);
        g.GetComponent<NetworkObject>().Spawn(true);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        AudioManager.instance.playClip(explostionClip);
    }

    void Update()
    {

    }

    public void setRadius(float radius)
    {

        this.radius = radius;
        GetComponent<SphereCollider>().radius = radius;
    }

}
