
using Unity.Netcode;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;



public class CTFGUIManager : MonoBehaviour
{
    static private NetworkManager m_NetworkManager;

    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    void OnGUI()
    {
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) m_NetworkManager.StartHost();
        if (GUILayout.Button("Client")) m_NetworkManager.StartClient();
        if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
    }

    static void StatusLabels()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.white;

        var gameManager = FindObjectOfType<CTFGameManager>();
        GUILayout.Label(String.Format("Score={0}", gameManager.m_score.Value), style);

        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);

        var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
        if(playerObject != null)
        {
            var manager = playerObject.GetComponent<CTFCubeManager>();
            GUILayout.Label(manager.m_guiMsg1);
            GUILayout.Label(manager.m_guiMsg2);
        }
    }
}
