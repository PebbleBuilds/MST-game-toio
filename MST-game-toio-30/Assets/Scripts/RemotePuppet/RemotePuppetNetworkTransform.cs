using System;
using Unity.Netcode;
using UnityEngine;
using toio;
using Cysharp.Threading.Tasks;

public class RemotePuppetNetworkTransform : NetworkBehaviour
{

    void Start()
    {
    }

    void Update()
    {
    }

    public void UpdateTransform(float x, float y, float angle)
    {
        transform.position = ToioHelpers.PositionIDtoUnity(x,y);
        transform.eulerAngles = new Vector3(0,angle,0);
    }
}