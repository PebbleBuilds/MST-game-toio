
using Unity.Netcode;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;



public class GUIManager : MonoBehaviour
{
    static private NetworkManager m_NetworkManager;

    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    void OnGUI()
    {
        
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            StartButtons();
            GUILayout.EndArea();
        }
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) m_NetworkManager.StartHost();
        if (GUILayout.Button("Client")) m_NetworkManager.StartClient();
        if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
    }
}
