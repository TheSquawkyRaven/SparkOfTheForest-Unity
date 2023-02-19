using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;


public interface IFire
{
    public GameObject gameObject { get; }
    public bool Extinguished { get; }
    public void DealDamage(int damage);
    public Vector3 Position { get; }
}

public class Player : MonoBehaviour, IFire
{

    public static Player Instance;

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
    public PlayerCamera PlayerCam;
    public Camera cam;

    public Reticle Reticle;
    public ParticleSystem FirePS;
    private ParticleSystem.EmissionModule FirePSEmission;
    public ParticleSystem DepositingPS;
    private ParticleSystem.EmissionModule DepositingPSEmission;

    public FirePointsUI FirePointsUI;
    public SparkExtinguished SparkExtinguished;

    public InputUI InputUI;

    public AudioSource Reignite;
    public AudioSource ShootA;
    public AudioSource Poof;
    public AudioSource OrbBeep;
    public AudioSource Click;

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
    private bool dead;
    public bool IsDead => dead;
    public bool Extinguished => dead;

    public Vector3 Position => TR.localPosition;

    private void Awake()
    {
        Instance = this;

        FirePSEmission = FirePS.emission;
        DepositingPSEmission = DepositingPS.emission;
    }

    private void Start()
    {
        Respawned(CampfireManager.GetFirstCampfire().TR.localPosition);
        SparkExtinguished.SetReigniteRequirement(RespawnFirePoints);
    }

    public void PlayClick()
    {
        Click.Play();
    }

    [ContextMenu("Reignite")]
    public void ReignitePlay()
    {
        Reignite.Play();
    }

    public void Respawned(Vector3 pos)
    {
        Reignite.Play();

        dead = false;
        SR.enabled = true;

        TR.localPosition = pos;
        PlayerCam.SetTrackPlayer();

        MovedUpdate();
        FirePointsUI.SetMaxFirePoints(MaxFirePoints);
        FirePointsUI.SetFirePoints(FirePoints);

        FirePoints = RespawnFirePoints;
        FirePointsUpdated();

        FirePS.Emit(100);
        FirePSEmission.rateOverTimeMultiplier = 1;
        DepositingPS.gameObject.SetActive(true);
        DepositingPSEmission.rateOverTimeMultiplier = 0;

        Cursor.visible = false;

    }

    public void Dead()
    {
        Poof.Play();

        dead = true;
        FirePoints = 0;
        SR.enabled = false;
        CampfireManager.PlayerDeadDetermineReigniteOrLose();

        FirePS.Emit(100);
        FirePSEmission.rateOverTimeMultiplier = 0;
        DepositingPS.gameObject.SetActive(false);
        DepositingPSEmission.rateOverTimeMultiplier = 0;

        Cursor.visible = true;
    }

    private void FirePointsUpdated()
    {
        if (dead)
        {
            return;
        }
        if (FirePoints <= 0)
        {
            Dead();
        }
        FirePointsUI.SetFirePoints(FirePoints);
        float scale = (float)FirePoints / MaxSizeFirePoints;
        CurrentSize = MinSize + (MaxSize - MinSize) * scale;
        TR.localScale = new(CurrentSize, CurrentSize, 1);

    }
    private void FlashFirePoints()
    {

    }

    public void DealDamage(int damage)
    {
        FirePoints -= damage;
        if (FirePoints < 0)
        {
            FirePoints = 0;
        }
        FirePointsUpdated();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead)
        {
            return;
        }
        if (collision.collider.CompareTag(CampfireTag))
        {
            InputUI.ShowDepositToCampfire(true);
            CampfireInRange = collision.collider.GetComponent<Campfire>();
        }
        if (FirePoints >= MaxFirePoints)
        {
            return;
        }
        else if (collision.collider.CompareTag(OrbTag))
        {
            OrbBeep.Play();
            Destroy(collision.gameObject);
            FirePoints += FirePointsPerOrb;
            FirePointsUpdated();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (dead)
        {
            return;
        }
        if (collision.collider.CompareTag(CampfireTag))
        {
            InputUI.ShowDepositToCampfire(false);
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
        PlaceCampfireUpdate();
    }

    private void MoveUpdate()
    {
        if (dead)
        {
            return;
        }
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
        if (dead)
        {
            return;
        }
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
        ShootA.Play();
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
        if (dead)
        {
            return;
        }
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
        //if (FirePoints <= RespawnFirePoints)
        //{
        //    //Don't deposit
        //    FlashFirePoints();
        //    return false;
        //}
        //int willHaveRemaining = FirePoints - firePoints;
        //if (willHaveRemaining < RespawnFirePoints)
        //{
        //    firePoints = FirePoints - RespawnFirePoints;
        //}
        if (FirePoints < firePoints)
        {
            firePoints = FirePoints;
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

    private void PlaceCampfireUpdate()
    {
        if (FirePoints < CampfireManager.CampfireFirePoints)
        {
            InputUI.ShowPlaceCampfire(false);
            return;
        }
        Collider2D surroundingCampfire = Physics2D.OverlapCircle(TR.localPosition, 2, 1 << 10);
        if (surroundingCampfire != null)
        {
            InputUI.ShowPlaceCampfire(false);
            return;
        }
        InputUI.ShowPlaceCampfire(true);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (FirePoints >= CampfireManager.CampfireFirePoints)
            {
                FirePoints -= CampfireManager.CampfireFirePoints;
                FirePointsUpdated();
                PlaceCampfire();
            }
            else
            {
                FlashFirePoints();
            }
        }
    }

    private void PlaceCampfire()
    {
        CampfireManager.CreateCampfire(TR.localPosition);
    }

}
