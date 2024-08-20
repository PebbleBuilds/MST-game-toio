using System;
using Unity.Netcode;
using UnityEngine;
using toio;

static public class CTFConfig
{
    static public int numPlayers = 3;
    static public float stretchMin = 100;
    static public float stretchMax = 200;
    static public float stretchScalingFactor = 50;

    static public int CalculateVibration(float stretch)
    {
        return (int) ((stretch - stretchMin) / (stretchMax - stretchMin) * stretchScalingFactor);
    }
}