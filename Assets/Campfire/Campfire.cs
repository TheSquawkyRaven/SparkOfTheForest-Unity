using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Campfire : MonoBehaviour, IFire
{

    public Transform TR;

    public float Radius;

    private const int treeLM = 1 << 8;

    public TextMeshPro PointsText;
    public GameObject MaxTextObj;

    public int MaxFirePoints;
    public int FirePoints;

    private CampfireManager CampfireManager;

    public bool Extinguished => FirePoints == 0;

    public Vector3 Position => TR.localPosition;

    public void Ignite(CampfireManager CampfireManager, int firePoints)
    {
        this.CampfireManager = CampfireManager;
        FirePoints = firePoints;
        PointsText.SetText(FirePoints.ToString());
    }

    private void Start()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(TR.localPosition, Radius, treeLM);
        for (int i = 0; i < colliders.Length; i++)
        {
            Destroy(colliders[i].gameObject);
        }
    }

    public void DealDamage(int damage)
    {
        LoseFirePoints(damage);
        CampfireManager.CampfireChanged();
    }

    //public bool RemoveRequireFirePoints(int firePoints)
    //{
    //    if (FirePoints >= firePoints)
    //    {
    //        FirePoints -= firePoints;
    //        PointsText.SetText(FirePoints.ToString());
    //        MaxTextObj.SetActive(false);

    //        if (FirePoints == 0)
    //        {
    //            CampfireManager.CampfireDestroyed(this);
    //        }
    //        return true;
    //    }
    //    return false;
    //}
    //Return is destroyed
    public bool LoseFirePoints(int firePoints)
    {
        FirePoints -= firePoints;
        if (FirePoints <= 0)
        {
            FirePoints = 0;
            CampfireManager.CampfireDestroyed(this);
            return true;
        }
        PointsText.SetText(FirePoints.ToString());
        MaxTextObj.SetActive(false);
        return false;
    }

    public int DepositFirePoints(int firePoints)
    {
        if (FirePoints >= MaxFirePoints)
        {
            FirePoints = MaxFirePoints;
            PointsText.SetText(FirePoints.ToString());
            MaxTextObj.SetActive(true);
            return firePoints;
        }
        int remainingToMax = MaxFirePoints - FirePoints;
        if (firePoints >= remainingToMax)
        {
            FirePoints = MaxFirePoints;
            PointsText.SetText(FirePoints.ToString());
            MaxTextObj.SetActive(true);
            return remainingToMax - firePoints;
        }
        FirePoints += firePoints;
        PointsText.SetText(FirePoints.ToString());
        MaxTextObj.SetActive(false);
        return 0;
    }

}
