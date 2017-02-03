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

    public static void FadeAlphaToTarget(GameObject gameObject, float fadeSpeed, float myTargetAlpha)
    {
        Color currentColor = gameObject.GetComponent<Renderer>().material.color;
        if (currentColor.a < myTargetAlpha)
        {
            currentColor.a += fadeSpeed * Time.deltaTime;
            if (currentColor.a > myTargetAlpha) currentColor.a = myTargetAlpha;
        }
        else if (currentColor.a > myTargetAlpha)
        {
            currentColor.a -= fadeSpeed * Time.deltaTime;
            if (currentColor.a < myTargetAlpha) currentColor.a = myTargetAlpha;
        }
        gameObject.GetComponent<Renderer>().material.color = currentColor;
    }

    public static void HandleCameraZoom(float deltaMagnitudeDiff, Camera inputCamera, float zoomSpeed, float minSize, float maxSize)
    {
        // If the camera is orthographic...
        // ... change the orthographic size based on the change in distance between the touches.
        //inputCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;

        inputCamera.orthographicSize = Mathf.SmoothDamp(inputCamera.orthographicSize, 
            (inputCamera.orthographicSize + deltaMagnitudeDiff * zoomSpeed), 
            ref zoomSpeed, Time.deltaTime);

        if (inputCamera.orthographicSize < minSize) 
        {
            inputCamera.orthographicSize = minSize;
        }
        else if (inputCamera.orthographicSize > maxSize)
        {
            inputCamera.orthographicSize = maxSize;
        }
    }

}
