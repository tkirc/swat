using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Authors: Marc Fedespiel
public abstract class Weapon : MonoBehaviour
{



    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected Transform muzzle;

    [Header("Stats")]
    [SerializeField] protected int ammo;
    [SerializeField] protected EWeapon identifier;

    void Awake()
    {

        ammo = 1;
    }
    void Start()
    {

    }

    protected void init()
    {

        ammo = stats.ammo;
        GetComponentInChildren<MeshCollider>().enabled = false;
    }

    void Update()
    {

    }

    public virtual void shoot()
    {

        if (isEmpty()) return;

        Instantiate(stats.projectilePrefab, muzzle.position, muzzle.rotation);

        GameObject muzzleFlash = Instantiate(stats.muzzleFlashPrefab, muzzle.position, muzzle.rotation);
        muzzleFlash.transform.SetParent(muzzle);
        muzzleFlash.AddComponent<Destructor>().setDuration(0.25f);

        ammo--;
    }

    public virtual void drop()
    {

        Rigidbody rb;
        if (!TryGetComponent(out rb))
        {

            rb = gameObject.AddComponent<Rigidbody>();

            transform.SetParent(null);
            gameObject.AddComponent<Destructor>().setDuration(5.0f);
        }


        GetComponentInChildren<MeshCollider>().enabled = true;
    }

    public bool isEmpty()
    {

        return ammo <= 0;
    }

    public EWeapon getIdentifier()
    {

        return identifier;
    }

}
