using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// Authors: Marc Fedespiel
public class Bat : WeaponNetwork
{

    private int swinging = 0;
    private float startTime;

    private GameObject transformHolder;
    [SerializeField] private Transform[] transforms;
    private float duration;
    [SerializeField] private float force;

    private Transform start;
    private Transform target;

    void Start()
    {

        duration = stats.cooldownTime * 0.1666f;
    }
    public void onPickup(GameObject playerObject)
    {


        Transform[] transforms = playerObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {

            if (t.name == "BatSwing")
            {

                transformHolder = t.gameObject;
                break;
            }
        }

        transforms = transformHolder.GetComponentsInChildren<Transform>();
        this.transforms = new Transform[3];
        this.transforms[0] = transformHolder.transform;
        for (int i = 1; i < 3; i++)
        {

            this.transforms[i] = transforms[i];
        }

    }
    public override void shoot()
    {

        shootServerRpc(GetComponent<IDHolder>().getClientID());
    }

    [ServerRpc]
    public override void shootServerRpc(ulong clientID)
    {

        start = transforms[0];
        target = transforms[1];
        startTime = Time.time;
        swinging = 1;
    }

    public override void drop()
    {

    }

    [ServerRpc]
    public override void dropServerRpc()
    {
    }

    void Update()
    {

        if (swinging == 0) return;

        float t = (Time.time - startTime) / (duration);

        transform.position = Vector3.Lerp(start.position, target.position, t);
        transform.rotation = Quaternion.Lerp(start.rotation, target.rotation, t);

        if (t >= 1.0)
        {

            if (swinging == 1)
            {

                start = transforms[0];
                target = transforms[1];

                swinging = 2;
            }
            else if (swinging == 2)
            {

                start = transforms[1];
                target = transforms[2];

                swinging = 3;
            }
            else if (swinging == 3)
            {

                start = transforms[2];
                target = transforms[1];

                swinging = 4;
            }
            else if (swinging == 4)
            {

                start = transforms[1];
                target = transforms[0];

                swinging = 5;
            }
            else if (swinging == 5)
            {

                swinging = 0;
            }

            startTime = Time.time;
        }

    }


    void OnTriggerEnter(Collider collider)
    {

        if (!IsHost) return;
        if (swinging == 0) return;

        if ((1 << collider.gameObject.layer) == LayerMask.GetMask("Player"))
        {

            collider.gameObject.GetComponent<PlayerMovement>().setPushed(true);

            Rigidbody rb = collider.attachedRigidbody;
            rb.velocity = Vector3.zero;
            Vector3 dir = transformHolder.transform.forward;
            dir.x = force * dir.x * Mathf.Cos(Mathf.PI * 0.25f);
            dir.z = force * dir.z * Mathf.Sin(Mathf.PI * 0.25f);
            dir.y = force * 0.5f * Mathf.PI * 0.25f;
            rb.AddForce(dir, ForceMode.Impulse);
        }

    }


    public bool isSwinging()
    {

        return swinging == 0;
    }

}
