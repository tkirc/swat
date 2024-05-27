using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

using Random = System.Random;

// Authors: Marc Fedespiel
public class WeaponSpawnerNetwork : NetworkBehaviour
{

    [SerializeField] private GameObject[] weaponPrefabs;
    private GameObject onlySpawnThisWeaponOverride;
    [SerializeField] private float delay;
    [SerializeField] private WeaponNetwork spawnedWeapon;

    Random random = new Random();

    void Awake()
    {


        //onlySpawnThisWeaponOverride = (GameObject)Resources.Load("WeaponsNetwork/Landmine");

    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }


    public void acitvate()
    {


        gameObject.SetActive(true);
        spawnWeapon();
    }


    void Start()
    {

        if (!IsHost) return;

        spawnWeapon();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {

        checkPlayerPickup(collider);
    }


    void OnTriggerStay(Collider collider)
    {

        checkPlayerPickup(collider);
    }

    private void checkPlayerPickup(Collider collider)
    {



        if (spawnedWeapon == null) return;


        if (1 << collider.gameObject.layer == LayerMask.GetMask("Player"))
        {


            WeaponHolder holder = collider.gameObject.GetComponent<WeaponHolder>();

            if (holder.canPickupWeapon())
            {

                holder.pickupWeapon(spawnedWeapon);
                spawnedWeapon.GetComponent<CapsuleCollider>().enabled = true;
                removeWeapon();
                StartCoroutine("spawnWeaponTimed");
            }

        }


    }



    public void removeWeapon()
    {

        spawnedWeapon = null;
    }

    public IEnumerator spawnWeaponTimed()
    {

        yield return new WaitForSeconds(delay);

        spawnWeapon();
    }

    [ServerRpc]
    public void spawnWeaponServerRpc()
    {

        spawnWeapon();
    }

    private void spawnWeapon()
    {


        int index = random.Next() % weaponPrefabs.Length;

        GameObject g;
        if (onlySpawnThisWeaponOverride != null)
        {

            g = Instantiate(onlySpawnThisWeaponOverride);
        }
        else
        {

            g = Instantiate(weaponPrefabs[index]);
        }
        g.GetComponent<NetworkObject>().Spawn(true);
        spawnedWeapon = g.GetComponent<WeaponNetwork>();
        g.transform.position = transform.position;

        g.GetComponent<CapsuleCollider>().enabled = false;
        CapsuleCollider[] colliders = g.GetComponentsInChildren<CapsuleCollider>();
        foreach (CapsuleCollider collider in colliders)
        {

            collider.enabled = false;
        }

        MeshCollider mc = g.GetComponentInChildren<MeshCollider>();
        if (mc != null)
        {
            mc.enabled = false;
        }
    }
}
