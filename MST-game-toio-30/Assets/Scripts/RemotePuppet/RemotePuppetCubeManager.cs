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

    public double m_maxSpeed = 50;
    public float m_vibrationTolerance = 30;

    public NetworkVariable<Vector2> m_puppetPosID(null, NetworkVariableReadPermission.Everyone, NetworkVariableReadPermission.Owner);
    Vector2 m_playerPosID;

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
            cm.cubes[1].idCallback.AddListener("RemotePuppetManager", OnPuppetUpdateID);
            await cm.cubes[1].ConfigIDNotification(10, Cube.IDNotificationType.OnChanged);
            cm.cubes[1].TurnLedOn(255,0,0,0);
            m_connected = true;
        }
    }

    void Update()
    {
        if (m_connected)
        {
            var managers = UnityEngine.Object.FindObjectsOfType<RemotePuppetCubeManager>();
            foreach (var manager in managers) 
            {
                if (manager != this)
                {
                    Vector2 partnerPosID = ToioHelpers.UnitytoPositionID(manager.transform.position);
                    m_guiMsg2 = String.Format("Partner pos=(x:{0}, y:{1})", partnerPosID.x, partnerPosID.y);

                    if(cm.synced) // if the cubes are ready to receive commands
                    {
                        // move the local puppet cube.
                        cm.handles[1].Move2Target(partnerPosID.x,partnerPosID.y,m_maxSpeed).Exec(); // assume only 2 players for now

                        // if the remote puppet cube is far from the local player cube
                        if (manager.m_puppetPosID != null)
                        {
                            if ((m_playerPosID - manager.m_puppetPosID).magnitude > m_vibrationTolerance)
                            {
                                
                            }
                        }
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

        m_playerPosID.x = c.pos.x;
        m_playerPosID.y = c.pos.y;
    }
    void OnPuppetUpdateID(Cube c)
    {
        m_puppetPosID.x = c.pos.x;
        m_puppetPosID.y = c.pos.y;
    }
}