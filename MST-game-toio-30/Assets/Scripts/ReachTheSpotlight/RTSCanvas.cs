using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections;

public class RTSCanvas : NetworkBehaviour
{
    public Text m_text;
    public Image m_background;
    NetworkVariable<FixedString128Bytes> m_textString = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<int> m_leaderID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<bool> m_animating = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    int m_playerID;

    void Start()
    {
        m_leaderID.Value = -1;
    }

    void Update()
    {
        if(m_leaderID.Value == (int)NetworkManager.Singleton.LocalClientId && !m_animating.Value)
        {
            var tempColor = m_background.color;
            tempColor.a = 0.0f;
            m_background.color = tempColor;
            m_text.text = "";
        }
        else
        {
            var tempColor = m_background.color;
            tempColor.a = 1.0f;
            m_background.color = tempColor;
            m_text.text = m_textString.Value.ToString();
        }
    }

    IEnumerator RoundStartMsg()
    {
        m_textString.Value = "Next round's leader is " + Config.ColorNameFromPlayerID(m_leaderID.Value);
        yield return new WaitForSeconds(5);
        m_textString.Value = "Round begins in 3";
        yield return new WaitForSeconds(1);
        m_textString.Value = "Round begins in 2";
        yield return new WaitForSeconds(1);
        m_textString.Value = "Round begins in 1";
        yield return new WaitForSeconds(1);
        m_textString.Value = "Follow the leader to your (invisible) spotlight!";
        m_animating.Value = false;
    }
    public void StartNewRound(int leaderID)
    {
        m_leaderID.Value = leaderID;
        m_animating.Value = true;
        StartCoroutine("RoundStartMsg");
    }

    IEnumerator VictoryMsg()
    {
        m_textString.Value = "Nice work! You scored a point!";
        yield return new WaitForSeconds(5);
        m_animating.Value = false;
    }

    public void RoundVictory()
    {
        m_animating.Value = true;
        StartCoroutine("VictoryMsg");
    }

    IEnumerator FailureMsg()
    {
        m_textString.Value = "Time's up!";
        yield return new WaitForSeconds(5);
        m_animating.Value = false;
    }

    public void RoundFailure()
    {
        m_animating.Value = true;
        StartCoroutine("FailureMsg");
    }

    public bool IsAnimating()
    {
        return m_animating.Value;
    }
}