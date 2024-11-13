using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class MSTCubeManager : NetworkBehaviour
{
    // Connection
    public ConnectType connectType = ConnectType.Real; 
    public NetworkVariable<int> m_playerID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private int m_numPlayers = Config.numPlayers;
    CubeManager cm;
    NetworkVariable<bool> m_connected = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // GUI
    public String m_guiMsg1 = "";
    public String m_guiMsg2 = "";

    // Toio
    public double m_maxSpeed = 100;
    public NetworkVariable<int> m_vibrationIntensity = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public bool m_vibrationToggle = true;
    public Vector2 m_playerPosID;
    ToioVibration m_playerVibration = new ToioVibration();
    ToioLight m_playerLight;

    // On screen avatar rendering
    Color m_color;
    public Renderer m_renderer;
    public Collider m_collider;
    public NetworkVariable<float> m_alpha = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    async void Start()
    {
        // Get PlayerID
        if (IsOwner)
        {
            m_playerID.Value = (int)NetworkManager.Singleton.LocalClientId;
        }

        // Set up the rendering stuff. Do this even for the non-owners
        m_color = Config.ColorFromPlayerID(m_playerID.Value);
        m_collider = GetComponent<Collider>();

        // Only do Toio connection stuff if we own this player object
        if (IsOwner)
        {
            m_playerLight = new ToioLight(m_color, 0.5f, 0.5f);
            m_guiMsg1 = String.Format("Client ID={0}", m_playerID.Value);
            cm = new CubeManager(connectType);
            await cm.MultiConnect(m_numPlayers);

            for(int i = 0; i<m_numPlayers; i++)
            {
                if(i == m_playerID.Value)
                {
                    cm.cubes[i].idCallback.AddListener("CTFManager", OnPlayerUpdateID);
                    await cm.cubes[i].ConfigIDNotification(10, Cube.IDNotificationType.OnChanged);
                    //cm.cubes[i].TurnLedOn(0,255,0,0);
                }
                else
                {
                    await cm.cubes[i].ConfigIDNotification(10, Cube.IDNotificationType.OnChanged);
                    Color puppetColor = Config.ColorFromPlayerID(i);
                    cm.cubes[i].TurnLedOn((int)(puppetColor.r*255),(int)(puppetColor.g*255),(int)(puppetColor.b*255),0);
                }
            }
            m_connected.Value = true;
        }
    }

    void Update()
    {
        // Render the on-screen cube
        var color = Config.ColorFromPlayerID(m_playerID.Value);
        color.a = m_alpha.Value;
        m_renderer.material.color = color;

        if (m_connected.Value && IsOwner && cm.synced)
        {
            m_guiMsg2 = String.Format("Unity Avatar Position x={0}, z={1}", transform.position.x, transform.position.z);

            // move the local puppet cubes.
            var managers = UnityEngine.Object.FindObjectsOfType<MSTCubeManager>();
            foreach (var manager in managers) 
            {
                if (manager != this)
                {
                    Vector2 partnerPosID = ToioHelpers.UnitytoPositionID(manager.transform.position);
                    cm.handles[manager.m_playerID.Value].Move2Target(partnerPosID.x,partnerPosID.y,m_maxSpeed).Exec();
                }
            }

            // render necessary vibrations.
            if(!m_vibrationToggle)
            {
                m_playerVibration.Stop(cm.cubes[m_playerID.Value]);
            }
            else
            {
                m_playerVibration.Vibrate(cm.cubes[m_playerID.Value], m_vibrationIntensity.Value);
            }

            // flash avatar toio light.
            m_playerLight.UpdateFlash(cm.cubes[m_playerID.Value]);
        }
    }

    void OnPlayerUpdateID(Cube c)
    {
        transform.position = ToioHelpers.PositionIDtoUnity(c.pos.x,c.pos.y);
        transform.eulerAngles = new Vector3(0,c.angle,0);

        m_playerPosID.x = c.pos.x;
        m_playerPosID.y = c.pos.y;
    }

    public bool IsConnected() {return m_connected.Value;}

    [ClientRpc]
    public void PulseClientRpc(float duration,int intensity)
    {
        m_playerVibration.Pulse(duration,intensity);
    }
}