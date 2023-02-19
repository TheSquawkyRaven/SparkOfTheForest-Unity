using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class CampfireManager : MonoBehaviour
{

    public List<Campfire> ActiveCampfires = new();
    public Player player;
    public SparkExtinguished SparkExtinguished;
    public FireLost FireLost;
    public GameObject CampfirePrefab;
    public ParticleSystem FirePS;

    public int CampfireFirePoints;

    private int selectingCampfireIndex;
    [System.NonSerialized] public Campfire SelectingCampfire;

    private void Start()
    {
        for (int i = 0; i < ActiveCampfires.Count; i++)
        {
            ActiveCampfires[i].Ignite(this, CampfireFirePoints);
        }
    }

    public void CampfireChanged()
    {
        SparkExtinguished.CampfireChanged();
    }

    public Campfire GetClosestCampfire(Vector2 pos)
    {
        if (ActiveCampfires.Count == 0)
        {
            return null;
        }
        Campfire campfire = ActiveCampfires[0];
        float distSqr = Vector2.SqrMagnitude((Vector2)campfire.TR.localPosition - pos);
        for (int i = 1; i < ActiveCampfires.Count; i++)
        {
            float dist = Vector2.SqrMagnitude((Vector2)campfire.TR.localPosition - pos);
            if (dist < distSqr)
            {
                campfire = ActiveCampfires[i];
                distSqr = dist;
            }
        }
        return campfire;
    }

    public Campfire GetFirstCampfire()
    {
        return ActiveCampfires[0];
    }

    public void CreateCampfire(Vector3 pos)
    {
        Campfire campfire = Instantiate(CampfirePrefab, pos, Quaternion.identity, transform).GetComponent<Campfire>();
        campfire.Ignite(this, CampfireFirePoints);
        ActiveCampfires.Add(campfire);

        FirePS.transform.localPosition = pos;
        FirePS.Emit(100);
    }

    private void SelectCampfire()
    {
        SelectingCampfire = ActiveCampfires[selectingCampfireIndex];
        SparkExtinguished.CampfireChanged();
    }

    public void SelectLatestCampfire()
    {
        selectingCampfireIndex = ActiveCampfires.Count - 1;
        SelectCampfire();
    }

    public void SelectCampfireNext(bool right)
    {
        selectingCampfireIndex += right ? 1 : -1;
        if (selectingCampfireIndex < 0)
        {
            selectingCampfireIndex = ActiveCampfires.Count - 1;
        }
        if (selectingCampfireIndex >= ActiveCampfires.Count)
        {
            selectingCampfireIndex = 0;
        }
        SelectCampfire();
    }

    public void ReigniteCampfire()
    {
        //SelectingCampfire;
        Campfire campfire = SelectingCampfire;
        Vector3 pos = campfire.TR.localPosition;
        player.Respawned(pos);
        if (!campfire.LoseFirePoints(player.RespawnFirePoints))
        {
            ActiveCampfires.Remove(campfire);
            ActiveCampfires.Add(campfire);
        }
    }

    public void PlayerDeadDetermineReigniteOrLose()
    {
        if (ActiveCampfires.Count != 0)
        {
            SparkExtinguished.Extinguished();
            return;
        }
        FireLost.Lose();    //lose game
    }

    public void CampfireDestroyed(Campfire campfire)
    {
        FirePS.transform.localPosition = campfire.TR.localPosition;
        FirePS.Emit(100);
        if (campfire == SelectingCampfire)
        {
            int initialIndex = ActiveCampfires.IndexOf(campfire);
            selectingCampfireIndex = initialIndex - 1;
        }
        ActiveCampfires.Remove(campfire);
        Destroy(campfire.gameObject);
        if (selectingCampfireIndex < 0 || selectingCampfireIndex >= ActiveCampfires.Count)
        {
            selectingCampfireIndex = ActiveCampfires.Count - 1;
        }
        if (ActiveCampfires.Count == 0)
        {
            //TODO LOSE

            if (player.IsDead)
            {
                SparkExtinguished.Disable();
                FireLost.Lose();
            }
            return;
        }
        SelectCampfire();
    }

    public void GiveUpLose()
    {
        SparkExtinguished.Disable();
        FireLost.Lose();
    }

    //Returns remaining
    public int GiveFirePointsToCampfire(GameObject campfireObj, int firePoints)
    {
        Campfire campfire = campfireObj.GetComponent<Campfire>();
        return campfire.DepositFirePoints(firePoints);
    }


}
