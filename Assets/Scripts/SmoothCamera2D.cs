using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour {
	private Vector2 velocity;
	public float smoothTime = 0.15f;
	public Transform target;

    float rightBound;
    float leftBound;
    float topBound;
    float bottomBound;

    Transform topLeft;
    Transform bottomRight;

    public void setTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Awake()
    {
        this.topLeft = GameObject.FindGameObjectWithTag("TopLeft").transform;
        if (this.topLeft == null)
            Debug.LogError("Couldn't find TopLeft gameObject!");
        this.bottomRight = GameObject.FindGameObjectWithTag("BottomRight").transform;
        if (this.bottomRight == null)
            Debug.LogError("Couldn't find BottomRight gameObject!");
        SetBounds();
    }

	void Update () {
		if (target != null) 
		{
			Vector3 pos = this.transform.position;
			pos.x = Mathf.SmoothDamp (pos.x, target.position.x, ref(velocity.x), smoothTime);
			pos.y = Mathf.SmoothDamp (pos.y, target.position.y, ref(velocity.y), smoothTime);
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
