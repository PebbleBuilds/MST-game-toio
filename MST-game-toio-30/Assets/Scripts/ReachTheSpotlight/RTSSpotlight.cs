using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class RTSSpotlight : NetworkBehaviour
{
    public Renderer m_renderer;
    int m_playerID;
    bool m_reached;
    public bool IsReached() { return m_reached; }

    
    void Start()
    {
        Vector3 scale = new Vector3(RTSConfig.spotlightScale, 0.1f, RTSConfig.spotlightScale);
        transform.localScale = scale;
    }

    void Update()
    {
    }

    public void SetPlayerID(int playerID)
    {
        m_playerID = playerID;
        var color = Config.ColorFromPlayerID(playerID);
        color.a=0.5f;
        m_renderer.material.color = color;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    
    void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            var go = collision.gameObject;
            var manager = go.GetComponent<MSTCubeManager>();
            if (manager != null)
            {
                int playerID = manager.m_playerID.Value;
                if (playerID == m_playerID)
                {
                    m_reached = true;
                }                
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsServer)
        {
            var go = collision.gameObject;
            var manager = go.GetComponent<MSTCubeManager>();
            if (manager != null)
            {
                int playerID = manager.m_playerID.Value;
                if (playerID == m_playerID)
                {
                    m_reached = false;
                }                
            }
        }
    }
}