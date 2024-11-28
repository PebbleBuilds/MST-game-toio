using System;
using Unity.Netcode;
using UnityEngine;
using toio;

public enum PuppetCollisionFeedbackType {
  NONE,
  VIBRATION,
  DIRECTIONAL
}; 

static public class Config
{
    // general stuff
    static public int numPlayers = 2;
    // puppet stuff
    static public bool connectToPuppets = true;
    static public int puppetSpeed = 100;
    static public float puppetCollisionTolerance = 10; 
    static public PuppetCollisionFeedbackType puppetCollisionFeedback = PuppetCollisionFeedbackType.NONE;
    static public float puppetCollisionGracePeriodSeconds = 1;

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
}