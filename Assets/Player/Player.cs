using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public const string CampfireTag = "Campfire";
    public const string OrbTag = "Orb";

    public Transform TR;
    public SpriteRenderer SR;

    public AnimAdv Anim;

    public GameObject FireShotObj;
    public Vector2 ShotOffset;

    public TreesManager TreesManager;
    public CampfireManager CampfireManager;

    public TerrainManager TerrainManager;
    public Camera cam;

    public Reticle Reticle;
    public ParticleSystem DepositingPS;
    private ParticleSystem.EmissionModule DepositingPSEmission;

    public FirePointsUI FirePointsUI;

    public float MinSize;
    public float MaxSize;
    public int MaxSizeFirePoints;

    private float CurrentSize;

    //UPGRADES
    public float MoveSpeed;
    public float shotSpeed;
    public int FirePointsPerShot;
    public int MaxFirePoints;

    public int RespawnFirePoints;   //Deposit Minimum fire points
    public int FirePoints;

    public float depositCampfireRate;
    public float depositCampfireInitialDelay;
    public int depositCampfireAmount;

    public int FirePointsPerOrb;

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
        FirePointsUpdated();
        DepositingPSEmission = DepositingPS.emission;
        DepositingPSEmission.rateOverTimeMultiplier = 0;

    }

    private void Start()
    {
        MovedUpdate();
        FirePointsUI.SetMaxFirePoints(MaxFirePoints);
        FirePointsUI.SetFirePoints(FirePoints);
    }

    private void FirePointsUpdated()
    {
        FirePointsUI.SetFirePoints(FirePoints);
        float scale = (float)FirePoints / MaxSizeFirePoints;
        CurrentSize = MinSize + (MaxSize - MinSize) * scale;
        TR.localScale = new(CurrentSize, CurrentSize, 1);
    }
    private void FlashFirePoints()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(CampfireTag))
        {
            CampfireInRange = collision.collider.GetComponent<Campfire>();
        }
        if (FirePoints >= MaxFirePoints)
        {
            return;
        }
        else if (collision.collider.CompareTag(OrbTag))
        {
            Destroy(collision.gameObject);
            FirePoints += FirePointsPerOrb;
            FirePointsUpdated();
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
        Vector3 movement = MoveSpeed * CurrentSize * Time.deltaTime * input;

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
                TryShootShot();
            }
        }
    }

    private void TryShootShot()
    {
        if (FirePoints >= FirePointsPerShot)
        {
            ShootShot();
            FirePoints -= FirePointsPerShot;
            FirePointsUpdated();
            return;
        }
        FlashFirePoints();
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
            if (TryDepositToCampfire(depositCampfireAmount))
            {
                DepositingPSEmission.rateOverTimeMultiplier = 1;
            }
            else
            {
                DepositingPSEmission.rateOverTimeMultiplier = 0;
            }
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
                if (TryDepositToCampfire(depositCampfireAmount))
                {
                    DepositingPSEmission.rateOverTimeMultiplier = depositCampfireMultiplier;
                }
                else
                {
                    DepositingPSEmission.rateOverTimeMultiplier = 0;
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            DepositingPSEmission.rateOverTimeMultiplier = 0;
        }
    }

    private bool TryDepositToCampfire(int firePoints)
    {
        if (FirePoints <= RespawnFirePoints)
        {
            //Don't deposit
            FlashFirePoints();
            return false;
        }
        int willHaveRemaining = FirePoints - firePoints;
        if (willHaveRemaining < RespawnFirePoints)
        {
            firePoints = FirePoints - RespawnFirePoints;
        }
        int remaining = CampfireInRange.DepositFirePoints(firePoints);
        if (remaining == firePoints)
        {
            //Not deposited (campfire full)
            return false;
        }
        FirePoints -= firePoints;
        FirePoints += remaining;
        FirePointsUpdated();
        return true;
    }

}
