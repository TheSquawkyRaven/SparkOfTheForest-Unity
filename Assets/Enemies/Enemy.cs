using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Transform TR;
    public Transform SpriteTR;
    public Collider2D CL;
    public SpriteRenderer SR;
    public float MoveSpeed;
    public Player Player => Player.Instance;

    public float MoveSwitchTime;

    public float DetectionRadius;

    public AudioSource Hiss;
    public CampfireManager CampfireManager => Player.CampfireManager;

    public AnimationCurve yCurve;

    public int health;
    public float DamageRadius;
    public int firePointsDamage;

    public ContactFilter2D DetectionCF;
    private Vector3 lockedDirection;
    public Vector3 toPos;
    private float moveTimeC;
    private bool moving;
    private float fireCheckTimeC;

    private Campfire SeekingCampfire;

    private List<Collider2D> fireCLs = new();
    private IFire TargetFire;
    private bool hasHitThisMove;

    private void Update()
    {
        MoveUpdate();
        DetermineNextTargetUpdate();
    }

    public void DealDamage(int damage)
    {
        Hiss.Play();

        moving = !moving;
        moveTimeC = 0;

        SR.color = Color.gray;
        CL.enabled = false;
        StartCoroutine(RestoreColor());

        health -= damage;
        if (health <= 0)
        {
            Destroy(TR.gameObject);
        }
    }

    private IEnumerator RestoreColor()
    {
        yield return new WaitForSeconds(0.5f);
        SR.color = Color.white;
        CL.enabled = true;
    }

    private void MoveUpdate()
    {
        moveTimeC += Time.deltaTime;
        if (moveTimeC > MoveSwitchTime)
        {
            moveTimeC = 0;
            moving = !moving;
            Vector3 direction = toPos - TR.localPosition;
            direction.Normalize();
            lockedDirection = direction;

            FaceTarget();

            hasHitThisMove = false;
        }
        if (moving)
        {
            Vector3 move = MoveSpeed * Time.deltaTime * lockedDirection;
            TR.localPosition += move;

            float scale = moveTimeC / MoveSwitchTime;
            float yDiff = yCurve.Evaluate(scale);
            SpriteTR.localPosition = new(0, yDiff);

            FaceTarget();

            if (scale > 0.5f && !hasHitThisMove)
            {
                hasHitThisMove = true;
                TryHitFires();
            }
        }
    }

    private void TryHitFires()
    {
        fireCLs.Clear();
        int c = Physics2D.OverlapCircle(TR.localPosition, DamageRadius, DetectionCF, fireCLs);
        for (int i = 0; i < c; i++)
        {
            if (!fireCLs[i].TryGetComponent(out IFire fire))
            {
                continue;
            }
            if (fire.Extinguished)
            {
                continue;
            }
            Hiss.Play();
            fire.DealDamage(firePointsDamage);
        }


    }

    private void DetermineNextTargetUpdate()
    {
        fireCheckTimeC += Time.deltaTime;
        if (fireCheckTimeC > MoveSwitchTime * 4)
        {
            fireCheckTimeC = 0;
            FindAnyFire();
        }

        try
        {
            if (TargetFire != null)
            {
                toPos = TargetFire.Position;
                return;
            }
        }
        catch (System.Exception)
        {
            TargetFire = null;
            FindAnyFire();
            DetermineNextTargetUpdate();
        }
        try
        {
            if (SeekingCampfire != null)
            {
                toPos = SeekingCampfire.Position;
                return;
            }
        }
        catch (System.Exception)
        {
            SeekingCampfire = null;
            FindAnyFire();
            DetermineNextTargetUpdate();
        }
        toPos = Player.Position;
    }

    private void FaceTarget()
    {
        SR.flipX = lockedDirection.x > 0;
    }

    private void FindAnyFire()
    {
        fireCLs.Clear();
        int c = Physics2D.OverlapCircle(TR.localPosition, DetectionRadius, DetectionCF, fireCLs);
        if (c == 0)
        {
            SeekingCampfire = CampfireManager.GetClosestCampfire(TR.localPosition);
            return;
        }
        IFire closestFire = fireCLs[0].GetComponent<IFire>();
        float distSqr = float.MaxValue;
        if (closestFire != null)
        {
            distSqr = Vector2.SqrMagnitude(closestFire.Position - TR.localPosition);
        }
        if (closestFire != null && closestFire.Extinguished)
        {
            closestFire = null;
        }

        for (int i = 1; i < c; i++)
        {
            if (!fireCLs[i].TryGetComponent(out IFire fire))
            {
                continue;
            }
            if (fire.Extinguished)
            {
                continue;
            }
            else if (closestFire == null)
            {
                closestFire = fire;
                continue;
            }

            float dist = Vector2.SqrMagnitude(closestFire.Position - TR.localPosition);
            if (dist < distSqr)
            {
                closestFire = fire;
                distSqr = dist;
            }
        }
        TargetFire = closestFire;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Orb"))
        {
            Hiss.Play();
            Destroy(collision.collider.gameObject);
        }
    }

}
