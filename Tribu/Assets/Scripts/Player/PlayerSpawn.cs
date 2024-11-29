using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public Transform[] spawnPoints;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            int index = (int)NetworkManager.Singleton.LocalClientId % spawnPoints.Length;
            transform.position = spawnPoints[index].position;
        }
    }
}
