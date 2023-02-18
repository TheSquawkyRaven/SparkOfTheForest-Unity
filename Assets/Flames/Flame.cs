using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{

    public Transform TR;
    public SpriteRenderer SR;

    private Vector3 from;
    private Vector3 to;
    private float speedTime;
    private float timeC;

    public void SetFromTo(Vector3 from, Vector3 to, float speedTime)
    {
        this.from = from;
        this.to = to;
        this.speedTime = speedTime;
        TR.localPosition = from;
    }

    private void Update()
    {
        timeC += Time.deltaTime;
        float scale = timeC / speedTime;
        if (scale < 0.2f)
        {
            float a = scale / 0.2f;
            Color c = Color.white;
            c.a = a;
            SR.color = c;
        }
        else if (scale > 0.8f)
        {
            float a = 1 - ((scale - 0.8f) / 0.2f);
            Color c = Color.white;
            c.a = a;
            SR.color = c;
        }
        if (scale > 1)
        {
            scale = 1;
            Destroy(gameObject);
        }
        TR.localPosition = Vector3.Lerp(from, to, scale);
    }

}
