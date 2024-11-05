using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using toio;
using Cysharp.Threading.Tasks;



public class RTSGameManager : NetworkBehaviour
{
    // game state
    public NetworkVariable<int> m_score;
    public NetworkVariable<int> m_leaderID;
    public Vector2[] m_spotlightLocations = new Vector2[Config.numPlayers];

    // canvas
    public RTSCanvas m_canvas;

    // logger
    public ToioLogger m_logger = new ToioLogger("RTS",Config.numPlayers);

    void Start()
    {
        m_score.Value = 0;
        m_leaderID.Value = -1;

        m_canvas = FindObjectOfType<RTSCanvas>();
    }

    void FixedUpdate()
    {
        // TODO: does this need to be IsServer?

        var playerList = FindObjectsOfType<MSTCubeManager>();
        // Manage logger
        if(!m_logger.IsLogging())
        {
            foreach(var player in playerList)
            {
                MSTCubeManager manager = player.GetComponent<MSTCubeManager>();
                if(manager.IsConnected())
                {
                    m_logger.AddToio(manager);
                }
            }
        }
        else
        {
            m_logger.WriteData();
        }
    }

    async void Update()
    {
        if (IsServer)
        {
            // If we're just starting or between rounds, pick the next leader
            if(m_leaderID.Value < 0 && !m_canvas.IsAnimating())
            {
                m_leaderID.Value = Random.Range(0,Config.numPlayers);
                m_canvas.SetLeaderID(m_leaderID.Value);
            }
        }
    }

    void OnApplicationQuit()
    {
        m_logger.Quit();
    }

    /*
    [ClientRpc]
    public void SetBlackoutPanelClientRpc(bool enabled, ClientRpcParams rpcParams = default)
    {
        foreach (var obj in m_blackoutPanelList)
        {
            var img = obj.GetComponent<Image>();
            if(img != null)
            {
                if(enabled)
                {
                    img.CrossFadeAlpha(1.0f, 0.5f);
                }
                else
                {
                    img.CrossFadeAlpha(0.0f, 0.5f);
                }
            }
            else
            {
                var txt = obj.GetComponent<Text>();
                if(enabled)
                {
                    txt.CrossFadeAlpha(1.0f, 0.5f);
                }
                else
                {
                    txt.CrossFadeAlpha(0.0f, 0.5f);
                }
            }
        }
    }
    */
}