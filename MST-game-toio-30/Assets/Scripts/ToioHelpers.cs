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
    static public float xMiddle = (minX+maxX)/2;
    static public float yMiddle = (minY+maxY)/2;

    static public Vector3 PositionIDtoUnity(float x, float y)
    {
        Vector3 v = new Vector3((x-xMiddle)/xRange*planeScale, 0, -(y-yMiddle)/yRange*planeScale);
        return v;
    }

    static public Vector2 UnitytoPositionID(Vector3 v)
    {
        return new Vector2(v.x/planeScale*xRange + xMiddle, -v.z/planeScale*yRange + yMiddle);
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
    StreamWriter writer;
    string[] headerList;
    CTFCubeManager[] managerList;
    bool m_logging = false;
    string path;

    public ToioLogger(string fileName, int num_players)
    {
        path = fileName + System.DateTime.UtcNow.ToString() + ".csv";
        writer = new StreamWriter(path,true);

        managerList = new CTFCubeManager[num_players];
        headerList = new string[num_players];
    }

    public void AddToio(CTFCubeManager manager)
    {
        if(managerList[manager.m_playerID.Value] == null)
        {
            string playerID = manager.m_playerID.Value.ToString();
            headerList[manager.m_playerID.Value] = ",PositionX"+playerID+",PositionY"+playerID+",Angle"+playerID;
            managerList[manager.m_playerID.Value] = manager;
            Debug.Log("Logger: Registered CTFCubeManager with playerID " + playerID);
        }
        else {return;}

        bool ready = true;
        foreach (var m in managerList)
        {
            if (m == null) {ready = false;}
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
        foreach (var manager in managerList)
        {
            if (manager == null)
            {
                Debug.Log("Logger: A manager reference is broken. Stopping logging.");
                m_logging = false;
                return;
            }

            data += "," + manager.transform.position.x.ToString();
            data += "," + manager.transform.position.z.ToString();
            data += "," + manager.transform.eulerAngles.y.ToString();
        }
        writer.WriteLine(data);
    }

    private void StartLogging()
    {
        string header_string = "Time";
        foreach(string h in headerList)
        {
            header_string += h;
        }
        writer.WriteLine(header_string);
        m_logging = true;
        Debug.Log("Logger: Starting logging to " + path);
    }

    public void Quit()
    {
        writer.Close();
    }
}