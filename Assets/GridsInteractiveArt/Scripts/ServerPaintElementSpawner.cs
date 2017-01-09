using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
 
public class ServerPaintElementManager : NetworkBehaviour
{
    public GameObject elementPrefab;
    private GameObject elementInstance;

    // Use this for initialization
    public override void OnStartServer()
    {
        //if (!isServer) return;

        GameObject servChar = (GameObject)Instantiate(elementPrefab, transform.position, new Quaternion());
        NetworkServer.Spawn(servChar);
        elementInstance = servChar;
    }

    public GameObject GetServerCharacterInstance()
    {
        return elementInstance;
    }

    void Update()
    {

    }
}