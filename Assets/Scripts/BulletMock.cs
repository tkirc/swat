using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Authors: Marc Fedespiel
public class BulletMock : MonoBehaviour
{

    [SerializeField] private float force;
    void Start()
    {

        GetComponent<Rigidbody>().AddForce(transform.forward * force);
    }

    void Update()
    {

        if (transform.position.z < Camera.main.transform.position.z)
        {

            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        int layer = (1 << collider.gameObject.layer);
        if (layer == LayerMask.GetMask("Wall"))
        {


            Destroy(gameObject);
        }
    }

}
