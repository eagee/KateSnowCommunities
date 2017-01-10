using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteBehavior  {

    public static void SetSpriteAlpha(GameObject gameObject, float alphaLevel)
    {
        Color currentColor = gameObject.GetComponent<Renderer>().material.color;
        currentColor.a = alphaLevel;
        gameObject.GetComponent<Renderer>().material.color = currentColor;
    }

    public static void FadeAlphaToTarget(GameObject gameObject, float myTargetAlpha)
    {
        Color currentColor = gameObject.GetComponent<Renderer>().material.color;
        if (currentColor.a < myTargetAlpha)
        {
            currentColor.a += 1.0f * Time.deltaTime;
            if (currentColor.a > myTargetAlpha) currentColor.a = myTargetAlpha;
        }
        else if (currentColor.a > myTargetAlpha)
        {
            currentColor.a -= 1.0f * Time.deltaTime;
            if (currentColor.a < myTargetAlpha) currentColor.a = myTargetAlpha;
        }
        gameObject.GetComponent<Renderer>().material.color = currentColor;
    }

}
