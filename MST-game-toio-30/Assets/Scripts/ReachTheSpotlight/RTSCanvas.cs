using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RTSCanvas : NetworkBehaviour
{
    public Text m_text;
    public Image m_background;
    string m_textString = "";
    NetworkVariable<int> m_leaderID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    bool m_animating;
    int m_playerID;

    void Start()
    {
        m_leaderID.Value = -1;
    }

    void Update()
    {
        if (m_animating)
        {
            StartCoroutine("AnimatedMsg");
        }
        else if(m_leaderID.Value == (int)NetworkManager.Singleton.LocalClientId)
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
            m_text.text = m_textString;
        }
    }

    IEnumerator AnimatedMsg()
    {
        m_textString = "Next round's leader is " + Config.ColorNameFromPlayerID(m_leaderID.Value);
        yield return new WaitForSeconds(5);
        m_textString = "Round begins in 3";
        yield return new WaitForSeconds(1);
        m_textString = "Round begins in 2";
        yield return new WaitForSeconds(1);
        m_textString = "Round begins in 1";
        yield return new WaitForSeconds(1);
        m_textString = "Follow the leader to your (invisible) spotlight!";
        m_animating = false;
    }

    public void SetLeaderID(int leaderID)
    {
        m_leaderID.Value = leaderID;
        m_animating = true;
    }

    public bool IsAnimating()
    {
        return m_animating;
    }
}