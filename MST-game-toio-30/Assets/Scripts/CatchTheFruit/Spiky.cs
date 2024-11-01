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
        var manager = go.GetComponent<MSTCubeManager>();
        var avatar = go.GetComponent<CTFAvatar>();
        if (manager != null && avatar.IsScoring())
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
                    var game = FindObjectOfType<CTFGameManager>();
                    game.m_bungeeList[playerID].GetComponent<Bungee>().BreakBungee();
                    game.m_logger.LogEvent("Bungee broken by spiky: playerID " + playerID.ToString())
                }
                else // if a spiky hits the body, only bungee 1 will break. merciful?
                {
                    var game = FindObjectOfType<CTFGameManager>();
                    if(game.m_bungeeList != null)
                    {
                        game.m_bungeeList[1].GetComponent<Bungee>().BreakBungee();
                        game.m_logger.LogEvent("Bungee broken by spiky: playerID " + "1")
                    }
                }
                NetworkObject.Despawn(true);
            }
        }
    }
}