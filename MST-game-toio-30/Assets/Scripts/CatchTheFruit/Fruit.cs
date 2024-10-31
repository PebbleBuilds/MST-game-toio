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
            
            transform.position += new Vector3(0,0,-(float)CTFConfig.fruitSpeed);
            //transform.Rotate(0.0f,0.5f,0.0f,Space.World);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var go = collision.gameObject;
        var manager = go.GetComponent<CTFCubeManager>();
        if (manager != null && manager.IsScoring())
        {
            int playerID = manager.m_playerID.Value;
            if (playerID == 0) { return; } // body can't eat fruit

            // stuff to do on client. play a vibration?
            if (playerID == (int)NetworkManager.Singleton.LocalClientId)
            {

            }

            if (IsServer)
            {
                var gameManager = FindObjectOfType<CTFGameManager>().GetComponent<CTFGameManager>();
                gameManager.m_score.Value = gameManager.m_score.Value + 1;
                NetworkObject.Despawn(true);
            }

        }
    }
}