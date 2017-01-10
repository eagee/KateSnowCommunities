using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PaletteSelectionScript : MonoBehaviour
{
    public List<Sprite> spriteShapes;
    public bool isSelected = false;
    public bool isVisible = true;
    public int myCurrentSprintShape;

    private float myTimer = 0f;

    // Use this for initialization
    void Start()
    {
        myCurrentSprintShape = 0;
        myTimer = 0f;
        SpriteBehavior.SetSpriteAlpha(this.gameObject, 0.0f);
    }

    
    void Update()
    {
        // If the shape isn't selected, target alpha will always be 0 (so it will be hidden)
        float myTargetAlpha = 0.0f;

        // Otherwise we'll display the shape and animate it
        if (isSelected && isVisible)
        {
            myTargetAlpha = 1.0f;
        }

        SpriteBehavior.FadeAlphaToTarget(this.gameObject, 4f, myTargetAlpha);


        GetComponent<SpriteRenderer>().sprite = spriteShapes[myCurrentSprintShape];
        //ChangePaletteShape();

        // if(isSelected)
        // {
        //     myTargetScale = new Vector3(3.0f, 3.0f, 3.0f);
        // }
        // else
        // {
        //     myTargetScale = new Vector3(2.0f, 2.0f, 2.0f);
        // }
        // 
        // Vector3 scale = this.gameObject.transform.localScale;
        // if (scale.x < myTargetScale.x) scale.x += 0.1f;
        // if (scale.y < myTargetScale.y) scale.y += 0.1f;
        // if (scale.x > myTargetScale.x) scale.x -= 0.1f;
        // if (scale.y > myTargetScale.y) scale.y -= 0.1f;
        // 
        // this.gameObject.transform.localScale = scale;
    }

    private void ChangePaletteShape()
    {
        myTimer += Time.deltaTime * 4.0f;
        if(myTimer > 1.0f)
        {
            myTimer = 0;
            GetComponent<SpriteRenderer>().sprite = spriteShapes[myCurrentSprintShape];
            myCurrentSprintShape++;
            if (myCurrentSprintShape >= spriteShapes.Count)
            {
                myCurrentSprintShape = 0;
            }
        }
    }
}
