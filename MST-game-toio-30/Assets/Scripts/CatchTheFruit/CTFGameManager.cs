using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;



public class CTFGameManager : NetworkBehaviour
{
    public GameObject m_fruitPrefab;
    public GameObject m_spikyPrefab;
    public int m_fruitSpawnPeriod = 3;
    private float m_lastFruitSpawnTime = 0;

    public NetworkVariable<int> m_score;

    public GameObject m_bungeePrefab;
    public GameObject[] m_bungeeList = new GameObject[Config.numPlayers];
    public GameObject[] m_blackoutPanelList;

    public ToioLogger m_logger = new ToioLogger("CTF",Config.numPlayers);

    void Start()
    {
        m_score.Value = 0;
    }

    void FixedUpdate()
    {
        var playerList = FindObjectsOfType<MSTCubeManager>();

        // Manage logger
        if(!m_logger.IsLogging())
        {
            foreach(var player in playerList)
            {
                MSTCubeManager manager = player.GetComponent<MSTCubeManager>();
                if(manager.IsConnected())
                {
                    m_logger.AddToio(manager);
                }
            }
        }
        else
        {
            m_logger.WriteData();
        }
    }

    void Update()
    {
        if (IsServer)
        {
            // Host blackout?
            foreach (var obj in m_blackoutPanelList)
            {
                obj.SetActive(false);
            }

            // Fruit/spiky spawning
            var currTime = Time.time;
            if (currTime - m_lastFruitSpawnTime > m_fruitSpawnPeriod)
            {
                var position = ToioHelpers.PositionIDtoUnity(Random.Range(ToioHelpers.minX, ToioHelpers.maxX), ToioHelpers.minY);
                GameObject prefabToSpawn;
                if (Random.Range(0.0f,1.0f) < CTFConfig.spikyChance)
                {
                    prefabToSpawn = m_spikyPrefab;
                    m_logger.LogEvent("Spawned spiky at " + position.x.ToString() + "," + position.z.ToString());
                }
                else
                {
                    prefabToSpawn = m_fruitPrefab;
                    m_logger.LogEvent("Spawned fruit at " + position.x.ToString() + "," + position.z.ToString());
                }
                var fruitInstance = Instantiate(prefabToSpawn, position, Quaternion.identity);
                var fruitInstanceNetworkObject = fruitInstance.GetComponent<NetworkObject>();
                fruitInstanceNetworkObject.Spawn();
                m_lastFruitSpawnTime = currTime;
            }

            var playerList = FindObjectsOfType<MSTCubeManager>();

            // Manage bungees from body to each head
            foreach (var body in playerList)
            {
                var bodyCubeManager = body.GetComponent<MSTCubeManager>();
                var bodyID = bodyCubeManager.m_playerID.Value;
                int bodyVibration = 0;
                if (bodyID == 0)
                {
                    foreach(var head in playerList)
                    {
                        var headCubeManager = head.GetComponent<MSTCubeManager>();
                        var headID = headCubeManager.m_playerID.Value;
                        if (headID != 0)
                        {
                            // Spawn bungee if it's missing
                            if(m_bungeeList[headID] == null)
                            {
                                m_bungeeList[headID] = Instantiate(m_bungeePrefab);
                                var bungeeNetworkObject = m_bungeeList[headID].GetComponent<NetworkObject>();
                                bungeeNetworkObject.Spawn();
                            }

                            // Get the bungee component
                            Bungee bungeeComponent = m_bungeeList[headID].GetComponent<Bungee>();
                            bungeeComponent.SetHeadID(headID);

                            // Find position of body and head
                            var pos1 = body.transform.position;
                            var pos2 = head.transform.position;

                            // Calculate stretch in toio coordinates
                            var pos1mat = ToioHelpers.UnitytoPositionID(pos1);
                            var pos2mat = ToioHelpers.UnitytoPositionID(pos2);
                            var stretch = (pos1mat - pos2mat).magnitude;

                            // Propagate to bungee object for rendering;
                            bungeeComponent.pos1 = pos1;
                            bungeeComponent.pos2 = pos2;
                            bungeeComponent.stretch.Value = stretch;

                            // Calculate head vibration
                            if(bungeeComponent.m_enabled.Value)
                            {
                                headCubeManager.m_vibrationIntensity.Value = CTFConfig.CalculateVibration(stretch);
                                bodyVibration += CTFConfig.CalculateVibration(stretch);
                                bungeeComponent.m_vibration = bodyVibration;
                            }
                            else
                            {
                                headCubeManager.m_vibrationIntensity.Value = 0;
                            }

                            // Set blackout panel
                            //SetBlackoutPanelClientRpc(false,new ClientRpcParams //blackouting off rn
                            //{
                            //    Send = new ClientRpcSendParams
                            //    {
                            //        TargetClientIds = new ulong[] { (ulong)headID }
                            //    }
                            //});
                        }
                    }

                    // Calculate body vibration
                    bodyCubeManager.m_vibrationIntensity.Value = bodyVibration;
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        m_logger.Quit();
    }

    [ClientRpc]
    public void SetBlackoutPanelClientRpc(bool enabled, ClientRpcParams rpcParams = default)
    {
        foreach (var obj in m_blackoutPanelList)
        {
            obj.SetActive(enabled);
        }
        
    }
}