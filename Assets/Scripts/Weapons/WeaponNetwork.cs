using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Diagnostics;

// Authors: Marc Fedespiel
public abstract class WeaponNetwork : NetworkBehaviour
{

    [SerializeField] protected Transform muzzle;
    [SerializeField] protected WeaponStats stats;
    [SerializeField] private EWeapon identifier;
    [SerializeField]
    protected NetworkVariable<int> ammo =
                                new NetworkVariable<int>(
                                    1,
                                    NetworkVariableReadPermission.Everyone,
                                    NetworkVariableWritePermission.Server
                                );


    void Start()
    {

        GetComponent<CapsuleCollider>().enabled = false;
    }

    protected void init()
    {

        if (!IsHost) return;

        ammo.Value = stats.ammo;
        GetComponentInChildren<MeshCollider>().enabled = false;
    }

    void Update()
    {

    }

    public abstract void shoot();

    [ServerRpc]
    public virtual void shootServerRpc(ulong clientID)
    {

    }



    public virtual void drop()
    {

        dropServerRpc();
    }

    [ServerRpc]
    public virtual void dropServerRpc()
    {

        transform.SetParent(null);
        gameObject.AddComponent<Rigidbody>();
        GetComponent<CapsuleCollider>().enabled = true;
        gameObject.AddComponent<Destructor>().setDuration(3.0f);
    }


    public virtual bool isEmpty()
    {

        return ammo.Value <= 0;
    }


    public EWeapon getIdentifier()
    {

        return identifier;
    }

}
