using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TouchAndMouseInput : MonoBehaviour {
    public LayerMask touchInputMask;
    public Camera inputCamera;
    public float m_minCameraSize = 3.0f;
    public float m_maxCameraSize = 12.0f;
    public float m_cameraZoomSpeed = 0.25f;

    private List<GameObject> touchList = new List<GameObject>();
    private GameObject[] oldTouchList;
    private RaycastHit hit;
    private Vector3 m_lastHitPoint;

    private void HandleMultiTouchInput(ref Touch touchOne, Vector2 touchZeroPrevPos, ref Touch touchZero)
    {
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // If the camera is orthographic...
        // ... change the orthographic size based on the change in distance between the touches.
        inputCamera.orthographicSize += deltaMagnitudeDiff * m_cameraZoomSpeed;
        if (inputCamera.orthographicSize < m_minCameraSize)
        {
            inputCamera.orthographicSize = m_minCameraSize;
        }
        else if (inputCamera.orthographicSize > m_maxCameraSize)
        {
            inputCamera.orthographicSize = m_maxCameraSize;
        }
    }

    private void HandleSingleTouchInput()
    {
        oldTouchList = new GameObject[touchList.Count];
        touchList.CopyTo(oldTouchList);
        touchList.Clear();

        //Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, touchInputMask))
        {
            GameObject recipient = hit.transform.gameObject;
            touchList.Add(recipient);
            TouchPhase phase = TouchPhase.Canceled;

            if (Input.GetMouseButtonDown(0))
            {
                phase = TouchPhase.Began;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                phase = TouchPhase.Ended;
            }
            else if (Input.touchCount > 0)
            {
                phase = Input.GetTouch(0).phase;
            }

            if (phase == TouchPhase.Began)
            {
                //print("OntouchDown");
                recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
            }
            else if (phase == TouchPhase.Moved)
            {
               // If a user had moved their touch space more than 1.5f in distance 
               // (to prevent a massive amount of hints per move)
                if(Mathf.Abs(m_lastHitPoint.x - hit.point.x) > 1.5f ||
                   Mathf.Abs(m_lastHitPoint.y - hit.point.y) > 1.0f )
                {
                    recipient.SendMessage("OnTouchMoved", hit.point, SendMessageOptions.DontRequireReceiver);
                    m_lastHitPoint = hit.point;
                }
            }
            else if (phase == TouchPhase.Ended)
            {
                //   print("OntouchUp");
                recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
            }
            else if (phase == TouchPhase.Canceled)
            {
                //print("OntouchExit");
                recipient.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
            }
        }
        // Determine which was touched before and aren't touched in our new list, and 
        // trigger an exit signal meaning that the user was still pressing when they lost focus
        // on the object.
        foreach (GameObject go in oldTouchList)
        {
            if (!touchList.Contains(go))
            {
                //print("OntouchExit");
                go.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (inputCamera == null) return;

        // If there's only a single touch or a user is releasing the mouse button
        // use mouse input controls to interact with the scene. Otherwise use touch
        // input controls.

        //Debug.Log("Number of touches: " + Input.touchCount.ToString());
        if (Input.touchCount >= 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            HandleMultiTouchInput(ref touchOne, touchZeroPrevPos, ref touchZero);
        }
        else if (Input.touchCount == 1 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            HandleSingleTouchInput();
        }

    }
}