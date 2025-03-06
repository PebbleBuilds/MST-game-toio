using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using toio;
using Cysharp.Threading.Tasks;
using System;



public class DemoGameManager : NetworkBehaviour
{
    // logger
    public ToioLogger m_logger = new ToioLogger("Demo",Config.numPlayers);

    void Start()
    {
    }

    void FixedUpdate()
    {
        // TODO: does this need to be IsServer?
        var playerList = FindObjectsOfType<MSTCubeManager>();
        // Manage logger
        if(!m_logger.IsLogging())
        {
            foreach(var player in playerList)
            {
                MSTCubeManager manager = player.GetComponent<MSTCubeManager>();
                if(manager.IsConnected())
                {
                    m_logger.AddToio(manager);
                }
            }
        }
        else
        {
            m_logger.WriteData();
        }
    }

    async void Update()
    {
        if (IsServer && m_logger.IsLogging()) // janky way of checking if all the toios are connected
        {
            
        }
    }

    void OnApplicationQuit()
    {
        m_logger.Quit();
    }
}