using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

// Authors: Marc Federspiel
public class Health : NetworkBehaviour
{

    public static int MAX_HP = 1;

    [SerializeField] private AudioClip deathClip;

    [SerializeField]
    private NetworkVariable<int> hp =
        new NetworkVariable<int>(1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    private List<GameObject> toIgnore;

    public delegate void DeathCallback(ulong clientID);
    public delegate void SuicideCallback();

    private DeathCallback onDeath;
    private SuicideCallback onSuicide;

    public void registerOnDeathCallback(DeathCallback callback)
    {
        onDeath += callback;
    }

    public void unregisterOnDeathCallback(DeathCallback callback)
    {

        onDeath -= callback;
    }



    public void registerOnSuicideCallback(SuicideCallback callback)
    {

        onSuicide += callback;
    }

    public void unregisterOnDeathCallback(SuicideCallback callback)
    {

        onSuicide -= callback;
    }


    void Awake()
    {

        toIgnore = new List<GameObject>();
    }





    void OnCollisionEnter(Collision collision)
    {

        if (toIgnore.Contains(collision.gameObject)) return;

        ulong shooterID = long.MaxValue;
        if (collision.gameObject.TryGetComponent<IDHolder>(out IDHolder i))
        {

            shooterID = i.getClientID();
        }

        if (collisionCheck(1 << collision.gameObject.layer, shooterID))
        {

            toIgnore.Add(collision.gameObject);
        }
    }

    void OnCollisionStay(Collision collision)
    {

        if (toIgnore.Contains(collision.gameObject)) return;

        ulong shooterID = long.MaxValue;
        if (collision.gameObject.TryGetComponent<IDHolder>(out IDHolder i))
        {

            shooterID = i.getClientID();
        }

        if (collisionCheck(1 << collision.gameObject.layer, shooterID))
        {

            toIgnore.Add(collision.gameObject);
        }
    }


    void OnTriggerEnter(Collider collider)
    {

        if (toIgnore.Contains(collider.gameObject)) return;

        ulong shooterID = long.MaxValue;
        if (collider.gameObject.TryGetComponent<IDHolder>(out IDHolder i))
        {

            shooterID = i.getClientID();
        }

        if (collisionCheck(1 << collider.gameObject.layer, shooterID))
        {

            toIgnore.Add(collider.gameObject);
        }
    }


    void OnTriggerStay(Collider collider)
    {

        if (toIgnore.Contains(collider.gameObject)) return;

        ulong shooterID = long.MaxValue;
        if (collider.gameObject.TryGetComponent<IDHolder>(out IDHolder i))
        {

            shooterID = i.getClientID();
        }

        if (collisionCheck(1 << collider.gameObject.layer, shooterID))
        {

            toIgnore.Add(collider.gameObject);
        }
    }

    private bool collisionCheck(int layer, ulong shooterID)
    {

        bool takesDamage = IsOwner && !isDead() && (layer == LayerMask.GetMask("Damaging"));

        if (!takesDamage) return false;

        takeDamageServerRpc(shooterID, NetworkManager.Singleton.LocalClientId);
        return true;
    }

    [ServerRpc]
    public void takeDamageServerRpc(ulong shooterID, ulong hitClientID)
    {

        hp.Value--;

        if (isDead())
        {

            onDeathClientRpc();

            if (shooterID <= ulong.MaxValue)
            {

                if (shooterID != hitClientID)
                {

                    onDeath?.Invoke(shooterID);
                }
                else
                {

                    onSuicide?.Invoke();
                }
            }
            else
            {

                onSuicide?.Invoke();
            }



            PlayerNetwork player = gameObject.GetComponent<PlayerNetwork>();
            player.GetComponent<PlayerInput>().disableBattleControls();
            player.disableRenderer();

            Transform cam = Camera.main.transform;
            player.transform.position = cam.position + new Vector3(0.0f, 0.0f, cam.forward.z * -10f);

        }
    }

    [ClientRpc]
    private void onDeathClientRpc()
    {

        AudioManager.instance.playClip(deathClip);
    }

    public bool isDead()
    {

        return hp.Value <= 0;
    }

    public void resetHealth()
    {

        hp.Value = MAX_HP;
        toIgnore = new List<GameObject>();
    }

}
