using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;



public class RemotePuppetManager : MonoBehaviour
{
    static private NetworkManager m_NetworkManager;
    public ConnectType connectType = ConnectType.Real; 
    CubeManager cm;
    bool m_connected = false;

    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    async void Start()
    {
        cm = new CubeManager(connectType);
        await cm.MultiConnect(2);

        // cubes[0] is the player cube
        cm.cubes[0].idCallback.AddListener("RemotePuppetManager", OnPlayerUpdateID);
        await cm.cubes[0].ConfigIDNotification(10, Cube.IDNotificationType.OnChanged);
        cm.cubes[0].TurnLedOn(0,255,0,0);

        // cubes[1] is the partner cube
        cm.cubes[1].TurnLedOn(255,0,0,0);

        m_connected = true;
    }


    void Update()
    {
        if (m_NetworkManager.IsClient && m_connected)
        {
            var puppetTransformList = UnityEngine.Object.FindObjectsOfType<RemotePuppetNetworkTransform>();
            foreach (var networkTransform in puppetTransformList) // assume only 2 players for now
            {
                if (networkTransform.gameObject != m_NetworkManager.SpawnManager.GetLocalPlayerObject())
                {
                    cm.navigators[1].Navi2Target(networkTransform.transform.position.x, networkTransform.transform.position.y); 
                }
            }
        }
    }

    void OnPlayerUpdateID(Cube c)
    {
        var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
        if(playerObject != null)
        {
            var networkTransform = playerObject.GetComponent<RemotePuppetNetworkTransform>();
            networkTransform.UpdateTransform(c.pos.x, c.pos.y, c.angle);
        }
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
        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}
