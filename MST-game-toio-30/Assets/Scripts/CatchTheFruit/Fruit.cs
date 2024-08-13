using System;
using Unity.Netcode;
using UnityEngine;

public class Fruit : NetworkBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        if (IsServer && ToioHelpers.UnitytoPositionID(transform.position).y > ToioHelpers.maxY)
        {
            NetworkObject.Despawn(true);
        }

        transform.position += new Vector3(0,0,-(float)0.005);
    }
}