using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{

    public Transform TR;
    public SpriteRenderer SR;
    public Collider2D CL;
    public float RotationOffset;
    public float Speed;

    public float AppearTime;
    public float DespawnTime;

    private Vector2 direction;
    private float aliveTimeC;

    public void SetShot(Vector2 pos, Vector2 direction)
    {
        this.direction = direction.normalized;

        TR.localPosition = pos;
        TR.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction) + RotationOffset);
        TR.localScale = Vector3.zero;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        Destroy(collision.gameObject);
        CL.enabled = false;
    }

    private void Update()
    {
        aliveTimeC += Time.deltaTime;

        if (aliveTimeC < AppearTime)
        {
            float scale = aliveTimeC / AppearTime;
            Color color = Color.white;
            color.a = scale;
            SR.color = color;
            TR.localScale = new(scale, scale, 1);
        }

        if (aliveTimeC > DespawnTime)
        {
            Destroy(gameObject);
        }
        TR.localPosition += Speed * Time.deltaTime * (Vector3)direction;
    }

}
