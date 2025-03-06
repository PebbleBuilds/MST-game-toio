
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;



public class GUIManager : MonoBehaviour
{
    static private NetworkManager m_NetworkManager;
    Scene m_scene;

    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
        m_scene = SceneManager.GetActiveScene();
    }

    void OnGUI()
    {
        
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUIStyle style = new GUIStyle();
            style.fontSize = 40;
            style.normal.textColor = Color.white;
            GUILayout.Label(m_scene.name, style);
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
