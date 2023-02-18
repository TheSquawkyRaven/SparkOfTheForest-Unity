using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class Tree : MonoBehaviour
{

    public int FirePoints;

    private int burnStage;

    private TreesManager TreesManager;
    private AnimAdv FireAnimAdv;

    private float fpIncreaseC;

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
        Destroy(FireAnimAdv);
        Destroy(this);
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
        if (burnStage == 0)
        {
            if (FirePoints > TreesManager.FirePointsToMidBurn)
            {
                burnStage++;
                BurnStageChanged();
            }
            return;
        }
        if (burnStage == 1)
        {
            if (FirePoints > TreesManager.FirePointsToFinalBurn)
            {
                burnStage++;
                BurnStageChanged();
                TrySpreadToOthers();
            }
            return;
        }
        if (burnStage == 2)
        {
            if (FirePoints > TreesManager.FirePointsToAsh)
            {
                burnStage++;
                BurnStageChanged();
                DropFireOrb();
            }
            return;
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
            Destroy(FireAnimAdv.gameObject);
            GetComponent<SpriteRenderer>().sprite = TreesManager.AshSprite;
        }
    }

    private void TrySpreadToOthers()
    {
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
