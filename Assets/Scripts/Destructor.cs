using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

// Authors: Marc Fedespiel
public class Destructor : NetworkBehaviour
{

    [SerializeField] private float duration = 1.0f;

    [SerializeField] private bool destructionByClientAllowed = false;
    void Start()
    {

        if (!destructionByClientAllowed) return;

        StartCoroutine("destruct");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsHost) return;

        StartCoroutine("destruct");
    }

    void Update()
    {

    }

    private IEnumerator destruct()
    {

        yield return new WaitForSeconds(duration);

        if (!destructionByClientAllowed)
        {

            GetComponent<NetworkObject>().Despawn();
        }

        Destroy(gameObject);
    }



    public void setDuration(float duration)
    {

        this.duration = duration;
    }

    public void setDestructableByClient(bool b)
    {

        destructionByClientAllowed = b;
    }
}
