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
    public GameObject[] m_partsToHide;
    
    void Start()
    {
        manager = GetComponent<MSTCubeManager>();
    }

    void Update()
    {
        if(RTSConfig.renderFollowerAvatars || IsOwner)
        {
            m_renderer.enabled = true;
            foreach(var go in m_partsToHide)
            {
                go.SetActive(true);
            }
        }
        else
        {
            m_renderer.enabled = false;
            foreach(var go in m_partsToHide)
            {
                go.SetActive(false);
            }
        }
    }
}