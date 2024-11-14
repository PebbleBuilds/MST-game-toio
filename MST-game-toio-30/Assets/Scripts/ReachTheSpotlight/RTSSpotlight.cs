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
    Vector3 m_pos;

    
    void Start()
    {
        Vector3 scale = new Vector3(RTSConfig.spotlightScale, 1.0f, RTSConfig.spotlightScale);
        transform.localScale = scale;
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
                    m_reached = true;
                }                
            }
        }
    }


    /*
    void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            var go = collision.gameObject;
            var manager = go.GetComponent<MSTCubeManager>();
            Debug.Log("collided");
            if (manager != null)
            {
                int playerID = manager.m_playerID.Value;
                Debug.Log(String.Format("spotlight {0} hit target {1}", m_playerID.Value,playerID));
                if (playerID == m_playerID.Value)
                {
                    Debug.Log(String.Format("player {0} reached", playerID));
                    m_reached = true;
                }                
            }
        }
    }
    */

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
                    Debug.Log(String.Format("player {0} unreached", playerID));
                    m_reached = false;
                }                
            }
        }
    }
    
}