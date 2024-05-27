using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats", order = 1)]
public class WeaponStats : ScriptableObject
{

    public int ammo;
    public GameObject muzzleFlashPrefab;
    public GameObject projectilePrefab;

    public float cooldownTime;

    public AudioClip shootSFX;
}
