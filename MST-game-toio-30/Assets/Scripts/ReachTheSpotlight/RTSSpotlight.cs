using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class RTSSpotlight : NetworkBehaviour
{
    public Renderer m_renderer;
    public NetworkVariable<int> m_playerID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    bool m_reached;
    public bool IsReached() { return m_reached; }
    RTSGameManager m_gameManager;
    Vector3 m_pos;

    
    void Start()
    {
        Vector3 scale = new Vector3(RTSConfig.spotlightScale, 1.0f, RTSConfig.spotlightScale);
        transform.localScale = scale;
        m_gameManager = FindObjectOfType<RTSGameManager>().GetComponent<RTSGameManager>();
    }

    void Update()
    {
        if(m_playerID.Value >= 0)
        {
            var color = Config.ColorFromPlayerID(m_playerID.Value);
            color.a=0.5f;
            m_renderer.material.color = color;
        }
    }

    void FixedUpdate()
    {
        transform.position = m_pos;
    }

    public void SetPlayerID(int playerID)
    {
        m_playerID.Value = playerID;
        Debug.Log(String.Format("spotlight {0} placed", playerID));
    }

    public void SetPosition(Vector3 pos)
    {
        m_reached = false;
        m_pos = pos;
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsServer)
        {
            var go = collision.gameObject;
            var manager = go.GetComponent<MSTCubeManager>();
            if (manager != null)
            {
                int playerID = manager.m_playerID.Value;
                if (playerID == m_playerID.Value)
                {
                    if(!m_reached) { m_gameManager.m_logger.LogEvent(String.Format("Player {0} reached their spotlight", playerID)); }
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
                if (playerID == m_playerID.Value)
                {
                    m_gameManager.m_logger.LogEvent(String.Format("Player {0} left their spotlight", playerID));
                    m_reached = false;
                }                
            }
        }
    }
    
}