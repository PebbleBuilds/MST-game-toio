using System;
using Unity.Netcode;
using UnityEngine;
using toio;

static public class CTFConfig
{
    // general stuff
    static public int numPlayers = 3;

    // stretching and reforming
    static public float reformMax = 20;
    static public float stretchMin = 75;
    static public float stretchMax = 150;
    static public float stretchScalingFactor = 20;

    // fruit / spiky stuff
    static public float fruitSpeed = 0.05f;
    static public float spikyChance = 0.2f;

    // vibration stuff
    static public int minVibration = 8;
    static public int CalculateVibration(float stretch)
    {
        if (stretch < stretchMin)
        {
            return 0;
        }
        return (int) ((stretch - stretchMin) / (stretchMax - stretchMin) * stretchScalingFactor + minVibration);
    }
}