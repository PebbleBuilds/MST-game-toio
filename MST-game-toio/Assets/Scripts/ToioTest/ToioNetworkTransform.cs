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
    public String debug = "Toio not connected";

    async void Start()
    {
        if (IsClient)
        {
            cm = new CubeManager(connectType);
            await cm.SingleConnect();
            cube = cm.cubes[0];
            cube.idCallback.AddListener("ToioNetworkTransform", OnUpdateID);
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
                
            }
        }
    }

    void OnUpdateID(Cube c)
    {
        debug = String.Format("pos=(x:{0}, y:{1}), angle={2}", c.pos.x, c.pos.y, c.angle);
        transform.position = ToioHelpers.PositionIDtoUnity(cube.x,cube.y);
        transform.Rotate(0,0,cube.angle);
    }
}