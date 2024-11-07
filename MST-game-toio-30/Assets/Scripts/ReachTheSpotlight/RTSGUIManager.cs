
using Unity.Netcode;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;



public class RTSGUIManager : MonoBehaviour
{
    static private NetworkManager m_NetworkManager;
    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }
    void OnGUI()
    {
        
        if (m_NetworkManager.IsClient || m_NetworkManager.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            RTSStatusLabels();
            GUILayout.EndArea();
        }
    }
    static void RTSStatusLabels()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.white;

        var gameManager = FindObjectOfType<RTSGameManager>();
        GUILayout.Label(String.Format("Score={0}", gameManager.m_score.Value), style);
        GUILayout.Label(String.Format("Time Left={0}", (int)gameManager.m_timeLeft.Value), style);

        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("Unity Time: " + Time.time.ToString());

        var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
        if(playerObject != null)
        {
            var manager = playerObject.GetComponent<MSTCubeManager>();
            GUILayout.Label(manager.m_guiMsg1);
            GUILayout.Label(manager.m_guiMsg2);
        }
    }
}
