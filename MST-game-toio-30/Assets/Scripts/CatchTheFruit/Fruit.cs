using System;
using Unity.Netcode;
using UnityEngine;

public class Fruit : NetworkBehaviour
{
    void Start()
    {
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            if (ToioHelpers.UnitytoPositionID(transform.position).y > ToioHelpers.maxY)
            {
                NetworkObject.Despawn(true);
            }
            
            transform.position += new Vector3(0,0,-(float)0.05);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var go = collision.gameObject;
        var manager = go.GetComponent<CTFCubeManager>();
        if (manager != null)
        {
            // stuff to do on client. play a vibration?
            if (manager.m_playerID.Value == (int)NetworkManager.Singleton.LocalClientId)
            {

            }

            if (IsServer)
            {
                NetworkObject.Despawn(true);
            }

        }
    }
}