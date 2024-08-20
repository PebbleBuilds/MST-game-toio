using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class CTFCubeManager : NetworkBehaviour
{

    public ConnectType connectType = ConnectType.Real; 
    public NetworkVariable<int> m_playerID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private int m_numPlayers = CTFConfig.numPlayers;
    CubeManager cm;

    bool m_connected = false;
    public String m_guiMsg1 = "";
    public String m_guiMsg2 = "";

    public double m_maxSpeed = 100;
    public int m_vibrationIntensity = 0;
    public bool m_vibrationToggle = true;

    Vector2 m_playerPosID;
    ToioVibration m_playerVibration = new ToioVibration();

    async void Start()
    {

        // Only try to connect to cubes if this is our PlayerObject.
        if (IsOwner)
        {
            m_playerID.Value = (int)NetworkManager.Singleton.LocalClientId;
            m_guiMsg1 = String.Format("Client ID={0}", m_playerID.Value);
            cm = new CubeManager(connectType);
            await cm.MultiConnect(m_numPlayers);

            for(int i = 0; i<m_numPlayers; i++)
            {
                if(i == m_playerID.Value)
                {
                    cm.cubes[i].idCallback.AddListener("CTFManager", OnPlayerUpdateID);
                    await cm.cubes[i].ConfigIDNotification(10, Cube.IDNotificationType.OnChanged);
                    cm.cubes[i].TurnLedOn(0,255,0,0);
                }
                else
                {
                    await cm.cubes[i].ConfigIDNotification(10, Cube.IDNotificationType.OnChanged);
                    cm.cubes[i].TurnLedOn(255,0,0,0);
                }
            }
            m_connected = true;

            if(m_playerID.Value == 0) {m_vibrationToggle = false;}
        }
    }

    void Update()
    {
        if (m_connected)
        {
            var managers = UnityEngine.Object.FindObjectsOfType<CTFCubeManager>();
            foreach (var manager in managers) 
            {
                if (manager != this)
                {
                    Vector2 partnerPosID = ToioHelpers.UnitytoPositionID(manager.transform.position);

                    if(cm.synced) // if the cubes are ready to receive CubeHandle commands
                    {
                        // move the local puppet cube.
                        cm.handles[manager.m_playerID.Value].Move2Target(partnerPosID.x,partnerPosID.y,m_maxSpeed).Exec();
                    }
                }
            }

            // render necessary vibrations.
            if(IsOwner && cm.synced)
            {
                if(m_vibrationToggle)
                {
                    if(m_vibrationIntensity == 0)
                    {
                        m_playerVibration.Stop();
                    }
                    else
                    {
                        m_playerVibration.Vibrate(cm.cubes[m_playerID.Value], m_vibrationIntensity);
                    }
                    
                }
            }
                        
        }
    }

    void OnPlayerUpdateID(Cube c)
    {
        transform.position = ToioHelpers.PositionIDtoUnity(c.pos.x,c.pos.y);
        transform.eulerAngles = new Vector3(0,c.angle,0);

        m_playerPosID.x = c.pos.x;
        m_playerPosID.y = c.pos.y;
    }
}