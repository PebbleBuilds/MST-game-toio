using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class RemotePuppetCubeManager : NetworkBehaviour
{
    public ConnectType connectType = ConnectType.Real; 
    CubeManager cm;

    public GameObject playerPrefab;
    GameObject m_playerObject;

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

        m_playerObject = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Update()
    {
        var puppetTransformList = UnityEngine.Object.FindObjectsOfType<RemotePuppetNetworkTransform>();
        foreach (var networkTransform in puppetTransformList) // assume only 2 players for now
        {
            if (networkTransform.gameObject != m_playerObject)
            {
                cm.navigators[1].Navi2Target(networkTransform.transform.position.x, networkTransform.transform.position.y); 
            }
        }
    }

    void OnPlayerUpdateID(Cube c)
    {
        if(m_playerObject != null)
        {
            var networkTransform = m_playerObject.GetComponent<RemotePuppetNetworkTransform>();
            networkTransform.UpdateTransform(c.pos.x, c.pos.y, c.angle);
        }
    }
}