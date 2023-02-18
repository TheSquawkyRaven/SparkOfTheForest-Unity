using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{

    private const string TreeTag = "Tree";
    private const string EnemyTag = "Enemy";

    public Transform TR;
    public SpriteRenderer SR;
    public Collider2D CL;

    private Player player;

    public float RotationOffset;
    public float Speed;

    public int ShotFirePointsRequired;
    public int FirePoints;


    public float AppearTime;
    public float DespawnTime;
    public float ShrinkTime;

    //UPGRADES
    public float ChanceToSpreadToTree;
    public int FirePointsSpreadToTree;  //Duplicated

    private Vector2 direction;
    private bool doShrink;
    private float aliveTimeC;
    private float shrinkTimeC;

    public void SetShot(Player player, Vector2 pos, Vector2 direction)
    {
        this.player = player;
        FirePoints = ShotFirePointsRequired;
        this.direction = direction.normalized;

        TR.localPosition = pos;
        TR.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction) + RotationOffset);
        TR.localScale = Vector3.zero;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TreeTag))
        {
            bool spreadToTree = Random.value < ChanceToSpreadToTree;
            if (!spreadToTree)
            {
                return;
            }
            player.TreesManager.SetTreeOnFire(collision.gameObject, FirePointsSpreadToTree);

            // Don't destroy, just spread fire
        }
        else if (collision.CompareTag(EnemyTag))
        {
            //TODO Inflict damage with FirePoints
            Destroy(gameObject);
        }
        if (FirePoints <= 0)
        {
            Destroy(gameObject);
        }
        doShrink = true;
        shrinkTimeC = 0;
    }

    private void Update()
    {
        aliveTimeC += Time.deltaTime;

        float scale = aliveTimeC / AppearTime;
        if (scale > 1)
        {
            scale = 1;
            SR.color = Color.white;
        }
        else
        {
            Color c = Color.white;
            c.a = scale;
            SR.color = c;
        }
        if (doShrink)
        {
            shrinkTimeC += Time.deltaTime;
            float targetScale = (float)FirePoints / ShotFirePointsRequired;
            float shrinkScale = shrinkTimeC / ShrinkTime;
            if (shrinkScale > 1)
            {
                shrinkScale = 1;
            }
            float currentScale = Mathf.Lerp(scale, targetScale, shrinkScale);
            TR.localScale = new(currentScale, currentScale, 1);
        }
        else
        {
            TR.localScale = new(scale, scale, 1);
        }

        if (aliveTimeC > DespawnTime)
        {
            Destroy(gameObject);
        }
        TR.localPosition += Speed * Time.deltaTime * (Vector3)direction;
    }

}
