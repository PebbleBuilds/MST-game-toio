using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;


public class CTFGameManager : NetworkBehaviour
{
    public GameObject m_fruitPrefab;
    public int m_fruitSpawnPeriod = 3;
    private float m_lastFruitSpawnTime = 0;

    public NetworkVariable<int> m_score;

    public GameObject m_bungeePrefab;
    public GameObject[] m_bungeeList = new GameObject[CTFConfig.numPlayers];

    void Start()
    {
        m_score.Value = 0;
    }

    void FixedUpdate()
    {
        if (IsServer)
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

            var playerList = FindObjectsOfType<CTFCubeManager>();

            // Manage bungees from body to each head
            foreach (var body in playerList)
            {
                var bodyCubeManager = body.GetComponent<CTFCubeManager>();
                var bodyID = bodyCubeManager.m_playerID.Value;
                float bodyStretch = 0;
                if (bodyID == 0)
                {
                    foreach(var head in playerList)
                    {
                        var headCubeManager = head.GetComponent<CTFCubeManager>();
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
                                bodyStretch += stretch;
                            }
                        }
                    }

                    // Calculate body vibration
                    bodyCubeManager.m_vibrationIntensity.Value = CTFConfig.CalculateVibration(bodyStretch);
                }
            }
        }

    }
}