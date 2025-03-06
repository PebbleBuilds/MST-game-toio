
using Unity.Netcode;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;



public class DemoGUIManager : MonoBehaviour
{
    static private NetworkManager m_NetworkManager;

    public bool m_renderAvatars = true;

    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }
    void OnGUI()
    {
        
        if (m_NetworkManager.IsClient || m_NetworkManager.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            
            // connection details
            var mode = m_NetworkManager.IsHost ?
                "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
            GUILayout.Label("Unity Time: " + Time.time.ToString());

            // get the cube manager
            var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
            if(playerObject != null)
            {
                var manager = playerObject.GetComponent<MSTCubeManager>();
                GUILayout.Label(manager.m_guiMsg1);
                GUILayout.Label(manager.m_guiMsg2);

                // local settings
                m_renderAvatars = GUILayout.Toggle(m_renderAvatars, "Render Avatars");
                manager.m_vibrateOnPuppetCollision = GUILayout.Toggle(manager.m_vibrateOnPuppetCollision, "Vibrate on Puppet Collision");
                GUILayout.Label("Collision Vibration Intensity: ");
                manager.m_puppetCollisionVibrationIntensity = (int)GUILayout.HorizontalSlider((float)manager.m_puppetCollisionVibrationIntensity, 0.0f, 100.0f);
                GUILayout.Label("Collision Tolerance: ");
                manager.m_puppetCollisionTolerance = GUILayout.HorizontalSlider(manager.m_puppetCollisionTolerance, 0.0f, 100.0f);
                GUILayout.Label("Puppet Speed: ");
                manager.m_puppetCollisionTolerance = GUILayout.HorizontalSlider(manager.m_puppetSpeed, 0.0f, 100.0f);
            }

            GUILayout.EndArea();
        }
    }
}
