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
    public GameObject[] m_spotlightList = new GameObject[Config.numPlayers];
    public NetworkVariable<float> m_timeLeft;
    public bool m_roundActive = false;

    // canvas
    public RTSCanvas m_canvas;

    // prefabs
    public GameObject m_spotlightPrefab;

    // logger
    public ToioLogger m_logger = new ToioLogger("RTS",Config.numPlayers);

    void Start()
    {
        // Initialize values
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
        if (IsServer && m_logger.IsLogging()) // janky way of checking if all the toios are connected
        {
            // Update game state if not in the middle of an animation
            if(!m_canvas.IsAnimating())
            {
                // Decrement time
                if(m_timeLeft.Value > 0.0f)
                {
                    m_timeLeft.Value -= Time.deltaTime;
                }

                // If between rounds...
                if(m_leaderID.Value < 0)
                {
                    // Decide a leader for the next round
                    m_leaderID.Value = Random.Range(0,Config.numPlayers);

                    // Start the new round on the canvas
                    m_canvas.StartNewRound(m_leaderID.Value);

                    // Decide the spotlight positions
                    for(int i=0;i<Config.numPlayers;i++)
                    {
                        // Instantiate if null
                        if(m_spotlightList[i] == null)
                        {
                            m_spotlightList[i] = Instantiate(m_spotlightPrefab);
                            var spotlightNetworkObject = m_spotlightList[i].GetComponent<NetworkObject>();
                            spotlightNetworkObject.Spawn();
                        }
                        var position = ToioHelpers.PositionIDtoUnity(Random.Range(ToioHelpers.minX, ToioHelpers.maxX), Random.Range(ToioHelpers.minY, ToioHelpers.maxY));
                        var spotlightComponent = m_spotlightList[i].GetComponent<RTSSpotlight>();
                        spotlightComponent.SetPlayerID(i);
                        spotlightComponent.SetPosition(position);
                    }

                    // Start the timer
                    m_timeLeft.Value = RTSConfig.roundTimeSeconds;
                }

                // Else (during a round)...
                else
                {
                    // Check if every spotlight is reached
                    bool victory = true;
                    for(int i=0;i<Config.numPlayers;i++)
                    {
                        var spotlightComponent = m_spotlightList[i].GetComponent<RTSSpotlight>();
                        if(!spotlightComponent.IsReached())
                        {
                            victory = false;
                        }
                    }

                    // If so, victory!
                    if(victory)
                    {
                        m_score.Value += 1;
                        m_canvas.RoundVictory();
                        m_leaderID.Value = -1; // this signals to start a new round when done animating
                    }

                    // Else, check if out of time, if so, failure
                    else if(m_timeLeft.Value <= 0.0f)
                    {
                        m_canvas.RoundFailure();
                        m_timeLeft.Value = 0.0f;
                        m_leaderID.Value = -1; // this signals to start a new round when done animating
                    }
                }
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