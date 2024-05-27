using Unity.Netcode;
using UnityEngine;

public class MolotovCocktailMock : WeaponNetwork
{
    [SerializeField] private GameObject fireEffect;

    public override void shoot()
    {

        transform.SetParent(null);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        Vector3 throwDirection = transform.forward + transform.up;
        float throwForce = 10f;
        rb.AddForce(throwDirection.normalized * throwForce, ForceMode.VelocityChange);
    }

    [ServerRpc]
    public override void shootServerRpc(ulong clientID)
    {
    }




    void OnCollisionEnter(Collision collision)
    {

        if (!IsHost) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {

            Explode();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {

            Rigidbody rb = GetComponent<Rigidbody>();
            float magnitude = rb.velocity.magnitude;
            Vector3 direction = rb.velocity.normalized;
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
            rb.velocity = direction * magnitude * 0.75f;
        }
    }

    void Explode()
    {

        GameObject g = Instantiate(fireEffect, transform.position, Quaternion.identity);

        g.GetComponent<IDHolder>().setClientID(GetComponent<IDHolder>().getClientID());
        g.GetComponent<NetworkObject>().Spawn(true);

        if (TryGetComponent<NetworkObject>(out NetworkObject n))
        {

            n.Despawn();
        }

        Destroy(gameObject);
    }

}
