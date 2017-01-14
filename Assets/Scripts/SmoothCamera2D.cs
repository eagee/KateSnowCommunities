using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SmoothCamera2D : MonoBehaviour {
	private Vector2 velocity;
	public float smoothTime = 0.15f;
    public float maxOrthographicSize = 12.02705f;
    public float minOrthographicSize = 3.0f;
    public bool isTakingScreenShot = false;
    public Transform connectedTarget;
    public Transform disconnectedTarget;
    public NetworkManager myNetworkManager;
    public Transform activeTarget;

    float rightBound;
    float leftBound;
    float topBound;
    float bottomBound;

    Transform topLeft;
    Transform bottomRight;

    private Vector3 myStartingConnectedPosition;

    private void setTarget(Transform newTarget)
    {
        activeTarget = newTarget;
    }

    public void MoveConnectedTarget(float xVelocity, float yVelocity)
    {
        if(GetComponent<Camera>().orthographicSize < maxOrthographicSize)
        { 
            Vector3 pos = connectedTarget.transform.position;
            pos.x += xVelocity;
            pos.y += yVelocity;
            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
            pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
            connectedTarget.transform.position = pos;
            this.transform.position = connectedTarget.position;
            ClampPosition();
        }
    }

    void Awake()
    {
        myStartingConnectedPosition = connectedTarget.position;

        myNetworkManager = FindObjectOfType<NetworkManager>();
        setTarget(disconnectedTarget);
        this.topLeft = GameObject.FindGameObjectWithTag("TopLeft").transform;
        if (this.topLeft == null)
            Debug.LogError("Couldn't find TopLeft gameObject!");
        this.bottomRight = GameObject.FindGameObjectWithTag("BottomRight").transform;
        if (this.bottomRight == null)
            Debug.LogError("Couldn't find BottomRight gameObject!");
        SetBounds();
    }

	void Update () {

        // Update our camera target based on whether we have a client connection or not *and* based on the platform
        if((myNetworkManager.IsClientConnected()) && (Application.platform != RuntimePlatform.WindowsPlayer) && (!isTakingScreenShot))
        {
            setTarget(connectedTarget);
        }
        else
        {
            setTarget(disconnectedTarget);
            SpriteBehavior.HandleCameraZoom(1.0f, GetComponent<Camera>(), 0.1f, minOrthographicSize, maxOrthographicSize);
        }

        if(GetComponent<Camera>().orthographicSize >= 12f)
        {
            connectedTarget.transform.position = myStartingConnectedPosition;
        }

		if (activeTarget != null) 
		{
			Vector3 pos = this.transform.position;
			pos.x = Mathf.SmoothDamp (pos.x, activeTarget.position.x, ref(velocity.x), smoothTime);
			pos.y = Mathf.SmoothDamp (pos.y, activeTarget.position.y, ref(velocity.y), smoothTime);
			this.transform.position = pos;
			ClampPosition ();
		}
	}

    private void SetBounds()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = (vertExtent * Screen.width / Screen.height);
        leftBound = (float)(topLeft.position.x + horzExtent);
        rightBound = (float)(bottomRight.position.x - horzExtent);
        bottomBound = (float)(bottomRight.position.y + vertExtent);
        topBound = (float)(topLeft.position.y - vertExtent);
    }

    private void ClampPosition()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, -10f);
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        transform.position = pos;
    }
}
