using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using System.IO;

static public class ToioHelpers
{
    static public float minX = 98;
    static public float maxX = 402;
    static public float minY = 142;
    static public float maxY = 358;
    static public float planeScale = 10;

    static public float xRange = maxX-minX;
    static public float yRange = maxY-minY;
    static public float planeResolutionXoverY = xRange / yRange;

    static public float xMiddle = (minX+maxX)/2;
    static public float yMiddle = (minY+maxY)/2;

    static public Vector3 PositionIDtoUnity(float x, float y)
    {
        Vector3 v = new Vector3((x-xMiddle)/xRange*planeScale*planeResolutionXoverY, 0, -(y-yMiddle)/yRange*planeScale);
        return v;
    }

    static public Vector2 UnitytoPositionID(Vector3 v)
    {
        return new Vector2(v.x/planeScale/planeResolutionXoverY*xRange + xMiddle, -v.z/planeScale*yRange + yMiddle);
    }
}

public class ToioVibration
{
    int dir = 1;
    bool pulsing = false;
    float pulseDuration;
    float pulseStartTime;
    int pulseIntensity;

    public void Vibrate(Cube c, int speed)
    {
        if(!pulsing)
        {
            c.Move(speed*dir,-speed*dir,0);
            dir = -dir;
        }
        else if (Time.time - pulseStartTime > pulseDuration)
        {
            pulsing = false;
        }
        else
        {
            c.Move(pulseIntensity*dir,-pulseIntensity*dir,0);
            dir = -dir;
        }
    }
    public void Stop(Cube c)
    {
        c.Move(0,0,0);
    }

    public void Pulse(float duration, int intensity)
    {
        pulseDuration = duration;
        pulseStartTime = Time.time;
        pulseIntensity = intensity;
        pulsing = true;
    }
}

public class ToioLight
{
    Color m_color;
    float m_secOn;
    float m_secOff;

    float m_lastOn = 0;
    float m_lastOff = 0;
    bool m_on = true;
    

    public ToioLight(Color color, float secOn, float secOff)
    {
        m_color = color;
        m_secOn = secOn;
        m_secOff = secOff;
    }

    public void UpdateFlash(Cube c)
    {
        float time = Time.time;
        if (m_on && time-m_lastOn > m_secOn)
        {
            c.TurnLedOff();
            m_lastOff = time;
            m_on = false;
        }
        else if (!m_on && time-m_lastOff > m_secOff)
        {
            c.TurnLedOn((int)(m_color.r*255),(int)(m_color.g*255),(int)(m_color.b*255),0);
            m_lastOn = time;
            m_on = true;
        }
    }
}

public class ToioLogger
{
    bool m_logging = false;

    // Trajectory Logging
    StreamWriter m_trajectoryWriter;
    string[] m_headerList;
    MSTCubeManager[] m_managerList;
    string m_trajectoryPath;

    // Game Event Logging
    StreamWriter m_eventWriter;
    string m_eventPath;



    public ToioLogger(string fileName, int num_players)
    {
        // Instantiate trajectory logger stuff
        m_trajectoryPath = "./MST_Data_Logs/" + fileName + "Trajectories" + System.DateTime.Now.ToString() + ".csv";
        m_managerList = new MSTCubeManager[num_players];
        m_headerList = new string[num_players];

        // Instantiate event logger stuff
        m_eventPath = "./MST_Data_Logs/" + fileName + "EventLog" + System.DateTime.Now.ToString() + ".mstlog";
    }

    public void AddToio(MSTCubeManager manager)
    {
        if(m_managerList[manager.m_playerID.Value] == null)
        {
            string playerID = manager.m_playerID.Value.ToString();
            m_headerList[manager.m_playerID.Value] = ",PositionX"+playerID+",PositionY"+playerID+",Angle"+playerID;
            m_managerList[manager.m_playerID.Value] = manager;
            Debug.Log("Logger: Registered MSTCubeManager with playerID " + playerID);
        }
        else {return;}

        bool ready = true;
        foreach (var m in m_managerList)
        {
            if (m == null) {ready = false;}
            else if (m.IsConnected() == false) {ready = false;}
        }
        if(ready) { StartLogging(); }
    }

    public bool IsLogging()
    {
        return m_logging;
    }

    public void WriteData()
    {
        if(!m_logging)
        {
            return;
        }

        string data = "";
        data += (Time.time).ToString();

        int counter = 0;
        foreach (var manager in m_managerList)
        {
            if (manager == null)
            {
                Debug.LogWarning("Logger: A manager reference is broken. Stopping logging.");
                m_logging = false;
                return;
            }
            if (manager.m_playerID.Value != counter)
            {
                Debug.LogWarning("Logger: Manager list is out of order. Did you make sure to call AddToio in order of playerID? Stopping logging.");
                m_logging = false;
                return;
            }
            counter += 1;

            data += "," + manager.transform.position.x.ToString();
            data += "," + manager.transform.position.z.ToString();
            data += "," + manager.transform.eulerAngles.y.ToString();
        }
        m_trajectoryWriter.WriteLine(data);
    }

    private void StartLogging()
    {
        // Open up the files
        m_trajectoryWriter = new StreamWriter(m_trajectoryPath,true);
        m_eventWriter = new StreamWriter(m_eventPath,true);

        string header_string = "Time";
        foreach(string h in m_headerList)
        {
            header_string += h;
        }
        m_trajectoryWriter.WriteLine(header_string);
        m_logging = true;
        Debug.Log("Logger: Starting trajectory logging to " + m_trajectoryPath);
        Debug.Log("Logger: Starting event logging to " + m_trajectoryPath);
    }

    public void LogEvent(string game_event, bool unity_log=false)
    {
        if (m_logging)
        {
            string time = (Time.time).ToString();
            m_eventWriter.WriteLine(time + ":" + game_event);
            if(unity_log)
            {
                Debug.Log("Event Log: " + game_event);
            }
        }
    }

    public void Quit()
    {
        if (m_trajectoryWriter != null) {m_trajectoryWriter.Close();}
        if (m_eventWriter != null) {m_eventWriter.Close();}
    }
}