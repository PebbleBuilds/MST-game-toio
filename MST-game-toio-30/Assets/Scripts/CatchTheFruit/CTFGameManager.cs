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

            foreach (var body in playerList)
            {
                var bodyID = body.GetComponent<CTFCubeManager>().m_playerID.Value;
                if (bodyID == 0)
                {
                    foreach(var head in playerList)
                    {
                        var headID = head.GetComponent<CTFCubeManager>().m_playerID.Value;
                        if (headID != 0)
                        {
                            if(m_bungeeList[headID] == null)
                            {
                                m_bungeeList[headID] = Instantiate(m_bungeePrefab);
                                var bungeeNetworkObject = m_bungeeList[headID].GetComponent<NetworkObject>();
                                bungeeNetworkObject.Spawn();
                            }

                            var pos1 = body.transform.position;
                            var pos2 = head.transform.position;

                            m_bungeeList[headID].transform.position = Vector3.Lerp(pos1,pos2,0.5f);
                            float angleWithY = Mathf.Atan((pos2.z-pos1.z)/(pos2.x-pos1.x));
                            var eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                            m_bungeeList[headID].transform.eulerAngles = eulerAngles;
                            m_bungeeList[headID].transform.Rotate(0.0f,angleWithY,0.0f,Space.World);
                            Debug.Log(angleWithY);
                            m_bungeeList[headID].transform.localScale = new Vector3(1,(pos2-pos1).magnitude/2,1);
                        }
                    }
                }
            }
        }

    }
}