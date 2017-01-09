using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PaintBulletScript : MonoBehaviour
{
    public List<Sprite> spriteShapes;
    public int myCurrentSprintShape;
    public float myScaleSpeed = 60.0f;
    Vector3 myTargetScale = new Vector3(1.62352f, 1.42581f, 0.17692f);

    // Use this for initialization
    void Start()
    {
        myCurrentSprintShape = 0;
    }

    void Update()
    {
        GetComponent<SpriteRenderer>().sprite = spriteShapes[myCurrentSprintShape];
        Vector3 scale = this.gameObject.transform.localScale;
        scale.x -= myScaleSpeed * Time.deltaTime;
        scale.y -= myScaleSpeed * Time.deltaTime;
        this.gameObject.transform.localScale = scale;
        if (scale.x < myTargetScale.x && scale.y < myTargetScale.y)
        {
            Destroy(this.gameObject);
        }
    }
}
