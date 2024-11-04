using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class CTFAvatar : NetworkBehaviour
{
    // MST Cube Manager
    MSTCubeManager manager;

    // List of avatar parts to hide on the body node
    public GameObject[] m_partsToHide;
    
    // Game-related
    private NetworkVariable<bool> m_scoring = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        manager = GetComponent<MSTCubeManager>();
    }

    void Update()
    {
        // Turn off the eyes and tongue if we are the body
        if (manager.m_playerID.Value == 0)
        {
            foreach(var go in m_partsToHide)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach(var go in m_partsToHide)
            {
                go.SetActive(true);
            }
        }
    }

    public void SetScoring(bool scoring)
    {
        m_scoring.Value = scoring;
        if(scoring)
        {
            manager.m_alpha.Value = 1.0f;
            manager.m_collider.enabled = true;
        }
        else
        {
            manager.m_alpha.Value = 0.0f;
            manager.m_collider.enabled = false;
        }
    }

    public bool IsScoring()
    {
        return m_scoring.Value;
    }
}