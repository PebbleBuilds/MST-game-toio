using System;
using Unity.Netcode;
using UnityEngine;
using toio;

static public class Config
{
    // general stuff
    static public int numPlayers = 3;

    // vibration stuff
    static public int minVibration = 8;

    // colors
    static public Color ColorFromPlayerID(int id)
    {
        if (id == 1) {return Color.blue;}
        else if (id == 2) {return Color.green;}
        else {return Color.white;} // Body color
    }
    static public string ColorNameFromPlayerID(int id)
    {
        if (id == 1) {return "Blue";}
        else if (id == 2) {return "Green";}
        else {return "White";} // Body color
    }

    // puppet handling (defaults)
    static public bool connectToPuppets = true;
    static public int puppetSpeed = 100;
    static public bool vibrateOnPuppetCollision = true;
    static public int puppetCollisionTolerance = 30;
    static public int puppetCollisionVibrationIntensity = 30;
}