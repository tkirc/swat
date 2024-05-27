using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using Unity.Netcode;
using UnityEngine;

using Random = System.Random;

// Authors: Marc Federspiel
public class OscelatingMovement : NetworkBehaviour
{

    [SerializeField] private Transform transformA;
    [SerializeField] private Transform transformB;
    [SerializeField] private BoxCollider killZoneTop;
    [SerializeField] private BoxCollider killZoneBottom;
    private BoxCollider boxCollider;
    private float thersholdToSquash;
    private float startTime;
    private Transform target;
    private Transform start;
    [SerializeField] private float travelDuration;
    private float elapsed;

    bool moving;

    Random random;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();

        boxCollider = GetComponent<BoxCollider>();
        thersholdToSquash = PlayerNetwork.localPlayer.GetComponent<CapsuleCollider>().radius;
        moving = false;
        disableKillZones();
        random = new Random();

        if (!IsHost) return;

        elapsed = 0.5f * travelDuration;
        random = new Random();
        int r = random.Next();
        start = (r & 0b1) == 1 ? transformA : transformB;
        target = (r & 0b1) == 1 ? transformB : transformA;
        calculatePosition();

    }

    void Start()
    {



    }

    void Update()
    {

        if (!IsHost || !moving) return;

        float t = calculatePosition();
        if (Mathf.Abs(transform.position.z - target.position.z) < boxCollider.size.z - thersholdToSquash)
        {

            if (target.position.z > transform.position.z)
            {

                killZoneTop.enabled = true;
            }
            else if (target.position.z < transform.position.z)
            {

                killZoneBottom.enabled = true;
            }
        }

        if (t >= 1.0f)
        {

            Transform temp = start;
            start = target;
            target = temp;
            startTime = Time.time;
            disableKillZones();
        }
    }

    private float calculatePosition()
    {

        float t = (Time.time - startTime) / travelDuration;

        float z = Mathf.SmoothStep(start.position.z, target.position.z, t);
        transform.position = new Vector3(transform.position.x, transform.position.y, z);

        return t;
    }

    public void activate()
    {

        if (!IsHost) return;

        startTime = Time.time - elapsed;
        moving = true;
    }

    public void deactivate()
    {
        if (!IsHost) return;

        elapsed = Time.time - startTime;
        moving = false;
        disableKillZones();
    }

    public void disableKillZones()
    {

        killZoneTop.enabled = false;
        killZoneBottom.enabled = false;
    }
}
