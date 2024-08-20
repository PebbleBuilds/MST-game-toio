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

                            // Find position of body and head
                            var pos1 = body.transform.position;
                            var pos2 = head.transform.position;

                            // Position and scale bungee object
                            m_bungeeList[headID].transform.position = Vector3.Lerp(pos1,pos2,0.5f);
                            float angleWithY = -Mathf.Atan((pos2.z-pos1.z)/(pos2.x-pos1.x)) / Mathf.PI * 180;
                            var eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                            m_bungeeList[headID].transform.eulerAngles = eulerAngles;
                            m_bungeeList[headID].transform.Rotate(0.0f,angleWithY,0.0f,Space.World);
                            m_bungeeList[headID].transform.localScale = new Vector3(1,(pos2-pos1).magnitude/2,1);

                            // Calculate stretch in toio coordinates
                            var pos1mat = ToioHelpers.UnitytoPositionID(pos1);
                            var pos2mat = ToioHelpers.UnitytoPositionID(pos2);
                            var stretch = (pos1mat - pos2mat).magnitude;
                            bodyStretch += stretch;

                            // Calculate head vibration
                            headCubeManager.m_vibrationIntensity = CTFConfig.CalculateVibration(stretch);
                            
                            // Change bungee colour if stretch above min
                            if (stretch > CTFConfig.stretchMax)
                            {
                                m_bungeeList[headID].GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                            }
                            else if (stretch > CTFConfig.stretchMin)
                            {
                                m_bungeeList[headID].GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                            }
                            else
                            {
                                m_bungeeList[headID].GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                            }
                        }
                    }

                    // Calculate body vibration
                    bodyCubeManager.m_vibrationIntensity = CTFConfig.CalculateVibration(bodyStretch);
                }
            }
        }

    }
}