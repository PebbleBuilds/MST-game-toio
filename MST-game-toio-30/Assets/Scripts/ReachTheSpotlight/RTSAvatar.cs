using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class RTSAvatar : NetworkBehaviour
{
    // MST Cube Manager
    MSTCubeManager manager;
    
    void Start()
    {
        manager = GetComponent<MSTCubeManager>();
    }

    void Update()
    {
    }
}