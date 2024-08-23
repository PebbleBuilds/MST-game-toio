using System;
using Unity.Netcode;
using UnityEngine;
using toio;

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
    }
    public void Stop(Cube c)
    {
        c.Move(0,0,0);
    }

    public void Pulse(float duration)
    {
        pulseDuration = duration;
        pulseStartTime = Time.time;
        pulsing = true;
    }
}