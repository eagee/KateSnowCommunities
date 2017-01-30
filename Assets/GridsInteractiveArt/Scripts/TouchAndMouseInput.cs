using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TouchAndMouseInput : MonoBehaviour {
    public LayerMask touchInputMask;
    public Camera inputCamera;
    public float m_minCameraSize = 3.0f;
    public float m_maxCameraSize = 12.0f;
    public float m_cameraZoomSpeed = 0.0005f;

    private NetworkManager m_networkManager;
    private List<GameObject> touchList = new List<GameObject>();
    private GameObject[] oldTouchList;
    private RaycastHit hit;
    private Vector3 m_lastHitPoint;

    void Awake()
    {
        m_networkManager = FindObjectOfType<NetworkManager>();
    }

    private void HandleTouchPanning(ref Touch touchZero, ref Touch touchOne)
    {
        float moveCameraX = 0f;
        float moveCameraY = 0f;

        if ((touchZero.deltaPosition.x < 0 && touchOne.deltaPosition.x < 0) || ((touchZero.deltaPosition.x > 0 && touchOne.deltaPosition.x > 0)))
        {
            moveCameraX = (Mathf.Min(touchZero.deltaPosition.x, touchOne.deltaPosition.x) * m_cameraZoomSpeed) * inputCamera.orthographicSize / 12f;
        }

        if ((touchZero.deltaPosition.y < 0 && touchOne.deltaPosition.y < 0) || ((touchZero.deltaPosition.y > 0 && touchOne.deltaPosition.y > 0)))
        {
            moveCameraY = (Mathf.Min(touchZero.deltaPosition.y, touchOne.deltaPosition.y) * m_cameraZoomSpeed) * inputCamera.orthographicSize / 12f;
        }

        SmoothCamera2D camera2d = inputCamera.GetComponent<SmoothCamera2D>();
        if ((moveCameraX != 0 || moveCameraY != 0) && (camera2d != null))
        {
            camera2d.MoveConnectedTarget(-moveCameraX, -moveCameraY);
        }
    }

    private void HandleTouchZooming(ref Touch touchZero, ref Touch touchOne)
    {
        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        SpriteBehavior.HandleCameraZoom(deltaMagnitudeDiff, inputCamera, m_cameraZoomSpeed, m_minCameraSize, m_maxCameraSize);
    }

    private void HandleMultiTouchInput(Touch touchZero, Touch touchOne)
    {
        HandleTouchPanning(ref touchZero, ref touchOne);
        HandleTouchZooming(ref touchZero, ref touchOne);
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

            HandleMultiTouchInput(touchZero, touchOne);
        }
        else if (Input.touchCount == 1 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            HandleSingleTouchInput();
        }

    }
}