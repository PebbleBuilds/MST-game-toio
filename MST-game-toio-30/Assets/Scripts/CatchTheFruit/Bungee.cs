using System;
using Unity.Netcode;
using UnityEngine;

public class Bungee : NetworkBehaviour
{
    public NetworkVariable<int> m_bungeeHeadID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public Renderer m_renderer;
    public NetworkVariable<bool> m_enabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public Vector3 pos1;
    public Vector3 pos2;
    public NetworkVariable<float> stretch = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> m_alpha = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public float m_reformRate = 0.01f;
    public float m_decayRate = 0.01f;

    void Start()
    {
        //m_enabled.Value = true;
        //m_renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Change bungee colour if stretch above min #TODO make this carry across to clients
        if (stretch.Value > CTFConfig.stretchMin)
        {
            m_renderer.material.SetColor("_Color", Color.yellow);
        }
        else
        {
            m_renderer.material.SetColor("_Color", Color.green);
        }

        // Change bungee alpha
        var color = m_renderer.material.color;
        color.a = m_alpha.Value;
        m_renderer.material.color = color;
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            // Position and scale bungee object
            transform.position = Vector3.Lerp(pos1,pos2,0.5f);
            float angleWithY = -Mathf.Atan((pos2.z-pos1.z)/(pos2.x-pos1.x)) / Mathf.PI * 180;
            var eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
            transform.eulerAngles = eulerAngles;
            transform.Rotate(0.0f,angleWithY,0.0f,Space.World);
            transform.localScale = new Vector3(1,(pos2-pos1).magnitude/2,1);

            if (m_enabled.Value)
            {
                // Break the bungee if stretchMax exceeded
                if (stretch.Value > CTFConfig.stretchMax)
                {
                    BreakBungee();
                }
            }

            else
            {
                // Reconnect the bungee if fully reformed
                if (m_alpha.Value >= 1.0f)
                {
                    ReformBungee();
                }
                // Otherwise, increment reform if close enough
                else if (stretch.Value < CTFConfig.reformMax)
                {
                    m_alpha.Value += m_reformRate;
                }
                // Otherwise, decrement reform if above zero
                else 
                {
                    if(m_alpha.Value - m_decayRate >= 0.0f)
                    {
                        m_alpha.Value -= m_decayRate;
                    }
                    else
                    {
                        m_alpha.Value = 0.0f;
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var go = collision.gameObject;
        var spiky = go.GetComponent<Spiky>();
        if (spiky != null)
        {
            if (IsServer)
            {
                BreakBungee();
                m_alpha.Value = 0.0f;
            }
        }
    }

    public void SetHeadID(int id)
    {
        m_bungeeHeadID.Value = id;
    }

    public void BreakBungee()
    {
        m_enabled.Value = false;
        m_alpha.Value = 0.0f;

        var managers = UnityEngine.Object.FindObjectsOfType<CTFCubeManager>();
        foreach (var manager in managers) 
        {
            if (manager.m_playerID.Value == m_bungeeHeadID.Value || manager.m_playerID.Value == 0)
            {
                manager.Pulse(0.5f,100);
            }
        }
    }

    public void ReformBungee()
    {
        m_enabled.Value = true;

        var managers = UnityEngine.Object.FindObjectsOfType<CTFCubeManager>();
        foreach (var manager in managers) 
        {
            if (manager.m_playerID.Value == m_bungeeHeadID.Value || manager.m_playerID.Value == 0)
            {
                manager.Pulse(0.5f,100);
            }
        }
    }
}