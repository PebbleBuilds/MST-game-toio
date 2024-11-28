using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class RTSAvatar : NetworkBehaviour
{
    // MST Cube Manager
    MSTCubeManager manager;
    RTSGameManager m_gameManager;
    public Renderer m_renderer;
    
    void Start()
    {
        manager = GetComponent<MSTCubeManager>();
        m_gameManager = GetComponent<RTSGameManager>();
    }

    void Update()
    {
        if(RTSConfig.renderFollowerAvatars || IsOwner)
        {
            m_renderer.enabled = true;
        }
        else
        {
            m_renderer.enabled = false;
            Debug.Log("asdf");
        }
    }
}