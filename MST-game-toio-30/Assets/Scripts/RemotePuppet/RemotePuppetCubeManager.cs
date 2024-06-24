using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class RemotePuppetCubeManager : NetworkBehaviour
{
    public ConnectType connectType = ConnectType.Real; 
    CubeManager cm;

    bool m_connected = false;
    public String m_guiMsg1 = "";
    public String m_guiMsg2 = "";

    async void Start()
    {
        // Only try to connect to cubes if this is our PlayerObject.
        if (IsOwner)
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
    }

    void Update()
    {
        if (m_connected)
        {
            var puppetTransformList = UnityEngine.Object.FindObjectsOfType<RemotePuppetCubeManager>();
            foreach (var networkTransform in puppetTransformList) // assume only 2 players for now
            {
                if (networkTransform != this)
                {
                    var xy = ToioHelpers.UnitytoPositionID(networkTransform.transform.position);
                    m_guiMsg2 = String.Format("Partner pos=(x:{0}, y:{1})", xy.Item1, xy.Item2);
                    if(cm.synced)
                    {
                        cm.handles[1].Move2Target(xy.Item1,xy.Item2).Exec(); 
                    }
                    
                }
            }
        }
    }

    void OnPlayerUpdateID(Cube c)
    {
        transform.position = ToioHelpers.PositionIDtoUnity(c.pos.x,c.pos.y);
        transform.eulerAngles = new Vector3(0,c.angle,0);
        m_guiMsg1 = String.Format("Player pos=(x:{0}, y:{1}, angle:{2})", c.pos.x,c.pos.y,c.angle);
    }
}