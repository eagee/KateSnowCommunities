using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// This script handles configuring each NetworkPlayer object when it's first spawned by the Network Manager.
/// </summary>
public class GridPlayerNetworkSetup : NetworkBehaviour {

    // Use this for initialization
    void Start () 
    {
        //SpriteBehavior.SetSpriteAlpha(this.gameObject, 0.0f);
    }


    //void Update()
    //{
    //    // If the shape isn't selected, target alpha will always be 0 (so it will be hidden)
    //    float myTargetAlpha = 1.0f;
    //    SpriteBehavior.FadeAlphaToTarget(this.gameObject, myTargetAlpha);
    //}

}
