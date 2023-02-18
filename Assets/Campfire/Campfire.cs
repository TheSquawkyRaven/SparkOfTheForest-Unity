using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Campfire : MonoBehaviour
{

    public Transform TR;

    public float Radius;

    private const int treeLM = 1 << 8;

    public TextMeshPro PointsText;
    public GameObject MaxTextObj;

    public int StartingFirePoints;
    public int MaxFirePoints;
    public int FirePoints;


    private void Awake()
    {
        FirePoints = 100;
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
