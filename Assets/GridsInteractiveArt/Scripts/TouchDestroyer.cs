using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


/// <summary>
///  Connected to Touch prefab object that is generated when a player touches the screen
///  this object will destroy itself if it touches an NPC object (triggering a state change)
/// </summary>
public class TouchDestroyer : NetworkBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "NPC")
        {
            print("Touching NPC and destroying self");
            print("Server TouchDestroyer");
            Network.Destroy(this.gameObject);
        }
    }
}
