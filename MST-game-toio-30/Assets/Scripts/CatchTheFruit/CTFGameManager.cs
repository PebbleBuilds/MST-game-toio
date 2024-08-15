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

    void Start()
    {
        m_score.Value = 0;
    }

    void Update()
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
        }

    }
}