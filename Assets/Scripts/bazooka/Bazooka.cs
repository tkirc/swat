using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

// Authors: Marc Federspiel
public class Bazooka : WeaponNetwork
{


    void Start()
    {
        init();

    }

    void Update()
    {

    }

    public override void shoot()
    {



        shootServerRpc(GetComponent<IDHolder>().getClientID());

        GameObject muzzleFlash = Instantiate(stats.muzzleFlashPrefab, muzzle.position, muzzle.rotation);
        Destructor d = muzzleFlash.AddComponent<Destructor>();
        d.setDuration(0.25f);
        d.setDestructableByClient(true);
    }

    [ServerRpc]
    public override void shootServerRpc(ulong clientID)
    {

        Vector3 direction = muzzle.position - transform.position;
        Ray ray = new Ray(transform.position, direction);

        GameObject g;
        if (Physics.Raycast(ray, out RaycastHit hit, direction.magnitude, LayerMask.GetMask("Wall")))
        {

            g = (GameObject)Instantiate(
                Resources.Load("WeaponsNetwork/BlastRadius"),
                hit.transform.position,
                Quaternion.identity);
        }
        else
        {

            g = Instantiate(stats.projectilePrefab, muzzle.position, muzzle.rotation);
        }

        ammo.Value--;
        g.GetComponent<NetworkObject>().Spawn(true);
        g.GetComponent<IDHolder>().setClientID(clientID);
    }

    [ClientRpc]
    public void shootClientRpc()
    {

        if (stats.shootSFX == null) return;

        AudioManager.instance.playClip(stats.shootSFX);
    }


}
