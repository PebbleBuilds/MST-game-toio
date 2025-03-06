using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class DemoAvatar : NetworkBehaviour
{
    // MST Cube Manager
    DemoGUIManager gui;
    public Renderer m_renderer;
    
    void Start()
    {
        gui = FindObjectOfType<DemoGUIManager>().GetComponent<DemoGUIManager>();
    }

    void Update()
    {
        if(gui.m_renderAvatars)
        {
            m_renderer.enabled = true;
        }
        else
        {
            m_renderer.enabled = false;
        }
    }
}