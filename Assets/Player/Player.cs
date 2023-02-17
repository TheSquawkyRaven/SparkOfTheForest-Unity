using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Transform TR;
    public SpriteRenderer SR;

    public AnimAdv Anim;
    public float MoveSpeed;

    public GameObject FireShotObj;
    public Vector2 ShotOffset;

    public TerrainManager TerrainManager;
    public Camera cam;

    public Reticle Reticle;

    public float shotSpeed;

    private float shotTimeC;

    private bool facingRight;
    private bool isMoving = false;

    private void Update()
    {
        MoveUpdate();

        Vector2 lookDirection = cam.ScreenToWorldPoint(Input.mousePosition) - TR.localPosition;
        if (lookDirection.x == 0)
        {

        }
        else if (lookDirection.x > 0)
        {
            SR.flipX = true;
            facingRight = true;
        }
        else
        {
            SR.flipX = false;
            facingRight = false;
        }

        if (!isMoving)
        {
            Anim.SetAnimSetIndex(0);
        }
        else
        {
            Anim.SetAnimSetIndex(1);
        }
        MovedUpdate();
        ShotUpdate();
    }

    private void MoveUpdate()
    {
        isMoving = false;
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            input.y += 1;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input.y -= 1;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input.x += 1;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            input.x -= 1;
            isMoving = true;
        }
        input.Normalize();
        Vector3 movement = MoveSpeed * Time.deltaTime * input;

        TR.localPosition += movement;
    }

    private void MovedUpdate()
    {
        TerrainManager.CameraUpdate(cam);
    }

    private void ShotUpdate()
    {
        shotTimeC += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if (shotTimeC > shotSpeed)
            {
                shotTimeC = 0;
                ShootShot();
            }
        }
    }

    private void ShootShot()
    {
        Shot shot = Instantiate(FireShotObj).GetComponent<Shot>();
        Vector2 shotOffset = ShotOffset;
        if (!facingRight)
        {
            shotOffset.x = -shotOffset.x;
        }
        shot.SetShot((Vector2)TR.localPosition + shotOffset, cam.ScreenToWorldPoint(Input.mousePosition) - TR.localPosition);

        Reticle.Recoil();
    }


}
