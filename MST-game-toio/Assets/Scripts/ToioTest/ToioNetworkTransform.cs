using System;
using Unity.Netcode;
using UnityEngine;
using toio;

public class ToioNetworkTransform : NetworkBehaviour
{
    public ConnectType connectType = ConnectType.Real; 
    CubeManager cm;
    Cube cube;
    bool connected = false;

    async void Start()
    {
        if (IsClient)
        {
            cm = new CubeManager(connectType);
            await cm.SingleConnect();
            cube = cm.cubes[0];
            await cube.ConfigIDNotification(10, Cube.IDNotificationType.OnChanged);

            Debug.Log("Toio Connected!");
            connected = true;
        }
    }

    void Update()
    {
        if (IsClient)
        {
            if (connected)
            {
                transform.position = ToioHelpers.PositionIDtoUnity(cube.x,cube.y);
                transform.Rotate(0,0,cube.angle);
            }
        }
    }
}