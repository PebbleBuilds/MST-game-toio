using System;
using Unity.Netcode;
using UnityEngine;
using toio;

static public class RTSConfig
{
    static public float spotlightScale = 3.0f;
    static public float pidb = spotlightScale/ToioHelpers.planeScale*ToioHelpers.yRange; // in position ID, minimum distance away from edge of mat to spawn spotlights
    static public float roundTimeSeconds = 30;
    static public bool renderFollowerAvatars = false;
}