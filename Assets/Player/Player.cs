using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public const string CampfireTag = "Campfire";

    public Transform TR;
    public SpriteRenderer SR;

    public AnimAdv Anim;
    public float MoveSpeed;

    public GameObject FireShotObj;
    public Vector2 ShotOffset;

    public TreesManager TreesManager;
    public CampfireManager CampfireManager;

    public TerrainManager TerrainManager;
    public Camera cam;

    public Reticle Reticle;
    public ParticleSystem DepositingPS;
    private ParticleSystem.EmissionModule DepositingPSEmission;

    public float shotSpeed;

    public int RespawnFirePoints;
    public int FirePoints;

    public float depositCampfireRate;
    public float depositCampfireInitialDelay;
    public int depositCampfireAmount;

    private float shotTimeC;

    private bool facingRight;
    private bool isMoving = false;

    private Campfire CampfireInRange;
    private float depositCampfireC;
    private int depositCampfireMultiplier;
    private bool depositCampfireInitialDelayed;

    private void Awake()
    {
        FirePoints = RespawnFirePoints;
        DepositingPSEmission = DepositingPS.emission;
        DepositingPSEmission.rateOverTimeMultiplier = 0;
    }

    private void Start()
    {
        MovedUpdate();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(CampfireTag))
        {
            CampfireInRange = collision.collider.GetComponent<Campfire>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(CampfireTag))
        {
            CampfireInRange = null;
        }
    }

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
        CampfireUpdate();
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
        shot.SetShot(this, (Vector2)TR.localPosition + shotOffset, cam.ScreenToWorldPoint(Input.mousePosition) - TR.localPosition);

        Reticle.Recoil();
    }


    private void CampfireUpdate()
    {
        if (CampfireInRange == null)
        {
            DepositingPSEmission.rateOverTimeMultiplier = 0;
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            depositCampfireInitialDelayed = false;
            depositCampfireC = 0;
            depositCampfireMultiplier = 1;
            CampfireInRange.DepositFirePoints(depositCampfireAmount);
            DepositingPSEmission.rateOverTimeMultiplier = 1;
        }
        if (Input.GetMouseButton(1))
        {
            depositCampfireC += Time.deltaTime;
            if (!depositCampfireInitialDelayed)
            {
                if (depositCampfireC > depositCampfireInitialDelay)
                {
                    depositCampfireC = 0;
                    depositCampfireInitialDelayed = true;
                }
            }
            else if (depositCampfireC > depositCampfireRate)
            {
                depositCampfireC = 0;
                depositCampfireMultiplier++;
                CampfireInRange.DepositFirePoints(depositCampfireAmount * depositCampfireMultiplier);
                DepositingPSEmission.rateOverTimeMultiplier = depositCampfireMultiplier;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            DepositingPSEmission.rateOverTimeMultiplier = 0;
        }
    }


}
