using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerPlayerSpawner : NetworkBehaviour {

    [SerializeField]
    public GameObject NetworkPlayerPrefab;
    public int numberOfBotsToSpawn = 1;

    [ServerCallback]
    void Start()
    {
        for (int i = 0; i < numberOfBotsToSpawn; i++)
        {
            Vector3 startPos = new Vector3(0f, 0f, 0f);
            Quaternion startRotation = new Quaternion();
            GameObject obj = Instantiate(NetworkPlayerPrefab, startPos, startRotation);
            obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
            
            obj.tag = "Bot";
            obj.name = "Bot" + i.ToString();
            NetworkServer.Spawn(obj);
        }
    }
}
