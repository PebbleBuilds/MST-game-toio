using System;
using Unity.Netcode;
using UnityEngine;

public class Spiky : NetworkBehaviour
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
            
            transform.position += new Vector3(0,0,-(float)CTFConfig.fruitSpeed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var go = collision.gameObject;
        var manager = go.GetComponent<CTFCubeManager>();
        if (manager != null && manager.IsScoring())
        {
            int playerID = manager.m_playerID.Value;

            // stuff to do on client. play a vibration?
            if (playerID == (int)NetworkManager.Singleton.LocalClientId)
            {
                //manager.PulseClientRpc(0.5f,100);
            }

            if (IsServer)
            {
                if (playerID != 0)
                {
                    var game = FindObjectOfType<CTFGameManager>().GetComponent<CTFGameManager>();
                    game.m_bungeeList[playerID].BreakBungee();
                }
                NetworkObject.Despawn(true);
            }
        }
    }
}