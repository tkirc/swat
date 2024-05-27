using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Authors: Marc Fedespiel
public class WeaponHolder : NetworkBehaviour
{

    [SerializeField] private WeaponNetwork bat;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private WeaponNetwork weapon;
    [SerializeField]
    private NetworkVariable<EWeapon> weaponType =
                            new NetworkVariable<EWeapon>(
                                EWeapon.NONE,
                                NetworkVariableReadPermission.Everyone,
                                NetworkVariableWritePermission.Server);

    private bool locked = false;

    void Awake()
    {

        weapon = bat;
    }
    public void enableWeaponPickup()
    {

        locked = false;
    }
    public void disableWeaponPickup()
    {

        locked = true;
    }

    public void pickupWeapon(WeaponNetwork weapon)
    {

        this.weapon = weapon;
        weaponType.Value = weapon.getIdentifier();
        weapon.transform.position = weaponTransform.position;
        weapon.transform.rotation = weaponTransform.rotation;
        weapon.transform.SetParent(transform);

        ulong clientID = GetComponent<IDHolder>().getClientID();
        weapon.GetComponent<IDHolder>().setClientID(clientID);

        if (weapon.TryGetComponent<Bat>(out Bat b))
        {

            bat = b;
        }
        else
        {

            bat.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }


    public void shoot()
    {


        weapon.shoot();

        if (weapon.isEmpty())
        {

            dropWeapon();
        }
        else if (weapon as MolotovCocktailMock != null || weapon as Landmine != null)
        {

            weapon = bat;
            bat.GetComponentInChildren<MeshRenderer>().enabled = true;
        }
    }


    public void dropWeapon()
    {


        weapon?.drop();
        bat.GetComponentInChildren<MeshRenderer>().enabled = true;
        weapon = bat;
    }


    public bool canPickupWeapon()
    {

        return !locked && weapon.getIdentifier() == EWeapon.BAT;
    }
}
