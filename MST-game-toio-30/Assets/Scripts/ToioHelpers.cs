using System;
using Unity.Netcode;
using UnityEngine;
using toio;

static public class ToioHelpers
{
    static float minX = 98;
    static float maxX = 402;
    static float minY = 142;
    static float maxY = 358;
    static float planeScale = 10;

    static float xRange = maxX-minX;
    static float yRange = maxY-minY;
    static float xMiddle = (minX+maxX)/2;
    static float yMiddle = (minY+maxY)/2;

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

    public void Vibrate(Cube c, int speed)
    {
        c.MoveRaw(speed*dir,-speed*dir,0);
        dir = -dir;
    }
    public void Stop(Cube c)
    {
        c.MoveRaw(0,0,0);
    }
}