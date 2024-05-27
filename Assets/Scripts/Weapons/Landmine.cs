using Unity.Netcode;
using UnityEngine;

// Authors: Thomas Kirchhofer
public class Landmine : WeaponNetwork
{
    [SerializeField] private GameObject blastRadiusPrefab;
    public override void shoot()
    {
        transform.SetParent(null);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        Vector3 vector = 2*transform.forward;
        rb.transform.position += vector;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")
            || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Explode();
        }
    }

    public override void OnNetworkSpawn()
    {
        
    }

    private void Explode()
    {
        GameObject g = Instantiate(blastRadiusPrefab, transform.position, transform.rotation);
        g.layer = LayerMask.NameToLayer("Damaging");
        g.GetComponent<NetworkObject>().Spawn(true);
        g.GetComponent<IDHolder>().setClientID(GetComponent<IDHolder>().getClientID());
        if (TryGetComponent<NetworkObject>(out NetworkObject n))
        {
            n.Despawn();
        }
        Destroy(gameObject);
    }
}
