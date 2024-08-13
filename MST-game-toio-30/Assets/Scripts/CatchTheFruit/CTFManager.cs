
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;



public class CTFManager : MonoBehaviour
{
    static private NetworkManager m_NetworkManager;

    public GameObject m_fruitPrefab;
    public int m_fruitSpawnPeriod = 3;
    private float m_lastFruitSpawnTime = 0;

    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    void Update()
    {
        if (m_NetworkManager.IsServer)
        {
            var currTime = Time.time;
            if (currTime - m_lastFruitSpawnTime > m_fruitSpawnPeriod)
            {
                var position = ToioHelpers.PositionIDtoUnity(Random.Range(ToioHelpers.minX, ToioHelpers.maxX), ToioHelpers.minY);
                var fruitInstance = Instantiate(m_fruitPrefab, position, Quaternion.identity);
                var fruitInstanceNetworkObject = fruitInstance.GetComponent<NetworkObject>();
                fruitInstanceNetworkObject.Spawn();

                m_lastFruitSpawnTime = currTime;
            }
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

        var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
        if(playerObject != null)
        {
            var manager = playerObject.GetComponent<CTFCubeManager>();
            GUILayout.Label(manager.m_guiMsg1);
            GUILayout.Label(manager.m_guiMsg2);
        }
    }
}
