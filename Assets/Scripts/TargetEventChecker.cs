using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEventChecker : MonoBehaviour
{
    public bool isDeath = false;

    public void setIsDeath(bool isOnIsland) { this.isDeath = isOnIsland; }

    public bool getIsDeath() { return this.isDeath; }


    private void OnCollisionEnter(Collision other)
    {

        layerCheck(other.gameObject.layer);
    }

    private void OnCollisionStay(Collision other)
    {

        layerCheck(other.gameObject.layer);
    }

    private void OnTriggerEnter(Collider other)
    {

        layerCheck(other.gameObject.layer);
    }


    private void OnTriggerStay(Collider other)
    {

        layerCheck(other.gameObject.layer);
    }

    private void layerCheck(int layer)
    {


        if (layer == LayerMask.NameToLayer("Damaging"))
        {
            setIsDeath(true);
        }
    }


}
