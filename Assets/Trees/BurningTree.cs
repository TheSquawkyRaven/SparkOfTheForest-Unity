using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class BurningTree : MonoBehaviour, IFire
{

    public int FirePoints;

    private int burnStage;

    private TreesManager TreesManager;
    private AnimAdv FireAnimAdv;

    private float fpIncreaseC;
    private bool hasSpread = false;

    public bool Extinguished => burnStage > 2;

    public Vector3 Position => transform.localPosition;

    //Entry
    public void SetOnFire(TreesManager TreesManager, AnimAdv fireAnimAdv, int firePoints)
    {
        this.TreesManager = TreesManager;
        FireAnimAdv = fireAnimAdv;
        FirePoints = firePoints;
        burnStage = 0;
    }

    public void SetExtinguished()
    {
        Destroy(FireAnimAdv.gameObject);
        Destroy(this);
    }

    public void DealDamage(int damage)
    {
        ReduceFirePoints(damage);
    }

    public void AddFirePoints(int firePoints)
    {
        if (burnStage > 2)
        {
            return;
        }
        FirePoints += firePoints;
    }
    public void ReduceFirePoints(int firePoints)
    {
        if (burnStage > 2)
        {
            return;
        }
        FirePoints -= firePoints;
        if (FirePoints <= 0)
        {
            SetExtinguished();
        }
    }

    private void Update()
    {
        if (burnStage > 2)
        {
            return;
        }
        FireUpdate();
        BurnStageUpdate();
    }

    private void FireUpdate()
    {
        fpIncreaseC += Time.deltaTime;
        if (fpIncreaseC > TreesManager.FirePointsIncreaseTime)
        {
            fpIncreaseC = 0;
            FirePoints++;
        }
    }

    private void BurnStageUpdate()
    {
        int intendedStage = 0;
        if (FirePoints > TreesManager.FirePointsToAsh)
        {
            intendedStage = 3;
        }
        else if (FirePoints > TreesManager.FirePointsToFinalBurn)
        {
            intendedStage = 2;
        }
        else if (FirePoints > TreesManager.FirePointsToMidBurn)
        {
            intendedStage = 1;
        }
        else if (FirePoints > 0)
        {
            intendedStage = 0;
        }

        if (intendedStage < burnStage)
        {
            burnStage = -1;
        }

        if (burnStage == -1)
        {
            burnStage++;
            BurnStageChanged();
        }
        if (burnStage == 0)
        {
            if (FirePoints > TreesManager.FirePointsToMidBurn)
            {
                burnStage++;
                BurnStageChanged();
            }
        }
        if (burnStage == 1)
        {
            if (FirePoints > TreesManager.FirePointsToFinalBurn)
            {
                burnStage++;
                BurnStageChanged();
                TrySpreadToOthers();
            }
        }
        if (burnStage == 2)
        {
            if (FirePoints > TreesManager.FirePointsToAsh)
            {
                burnStage++;
                BurnStageChanged();
                DropFireOrb();
            }
        }
    }

    private void BurnStageChanged()
    {
        if (burnStage <= 2)
        {
            FireAnimAdv.SetAnimSetIndex(burnStage);
        }
        else
        {
            FireLost.treesDestroyed++;
            Destroy(FireAnimAdv.gameObject);
            GetComponent<SpriteRenderer>().sprite = TreesManager.AshSprite;
        }
    }

    private void TrySpreadToOthers()
    {
        if (hasSpread)
        {
            return;
        }
        hasSpread = true;

        List<Collider2D> treesCL = TreesManager.TreeColliders;
        treesCL.Clear();
        int c = Physics2D.OverlapCircle(transform.localPosition, TreesManager.FireSpreadRadius, TreesManager.TreeCF, treesCL);
        for (int i = 0; i < c; i++)
        {
            bool canSpread = Random.value < TreesManager.FireSpreadChance;
            if (!canSpread)
            {
                continue;
            }
            TreesManager.FireSpreadToTree(this, treesCL[i], 1);
        }
    }

    private void DropFireOrb()
    {
        TreesManager.DropFireOrb(this);
    }

}
