using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Authors: Marc Fedespiel
public class GunMock : Weapon
{


    void Start()
    {

    }

    void Update()
    {

    }

    public override void shoot()
    {

        GameObject g = Instantiate(stats.projectilePrefab, muzzle.position, muzzle.rotation);
        g.layer = LayerMask.NameToLayer("Damaging_Ignore");
        GameObject muzzleFlash = Instantiate(stats.muzzleFlashPrefab, muzzle.position, muzzle.rotation);
        muzzleFlash.transform.SetParent(muzzle);
        muzzleFlash.AddComponent<Destructor>().setDuration(0.25f);
    }

}
