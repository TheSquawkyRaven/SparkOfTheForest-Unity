using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SparkExtinguished : MonoBehaviour
{

    public Animator Animator;
    public CampfireManager CampfireManager;
    public Player player;
    public GameObject NotEnoughFireObj;
    public Button ReigniteButton;
    public TextMeshProUGUI ReigniteText;

    public CanvasGroup CG;

    public float TimeToDisable;
    private float timeC;
    private bool disabled;


    public void Disable()
    {
        disabled = true;
        CG.interactable = false;
        CG.blocksRaycasts = true;
    }

    private void Update()
    {
        if (!disabled)
        {
            return;
        }
        timeC += Time.deltaTime;
        float scale = timeC / TimeToDisable;
        if (scale > 1f)
        {
            scale = 1;
            disabled = false;
        }
        CG.alpha = 1 - scale;
    }

    public void SetReigniteRequirement(int firePoints)
    {
        ReigniteText.SetText("REIGNITE (" + firePoints + ")");
    }

    public void Extinguished()
    {
        Animator.SetTrigger("Start");
    }

    private void Reignited()
    {
        Animator.SetTrigger("Stop");

        CampfireManager.ReigniteCampfire();
    }

    public void EventStartSelectCampfire()
    {
        CampfireManager.SelectLatestCampfire();
    }

    public void CampfireChanged()
    {
        Campfire campfire = CampfireManager.SelectingCampfire;
        if (campfire == null)
        {
            return;
        }

        if (player.IsDead)
        {
            player.PlayerCam.SetTrackCampfire(campfire);
        }
        if (campfire.FirePoints >= player.RespawnFirePoints)
        {
            NotEnoughFireObj.SetActive(false);
            ReigniteButton.interactable = true;
        }
        else
        {
            NotEnoughFireObj.SetActive(true);
            ReigniteButton.interactable = false;
        }
    }

    public void Left()
    {
        CampfireManager.SelectCampfireNext(false);
    }
    public void Right()
    {
        CampfireManager.SelectCampfireNext(true);
    }

    public void Reignite()
    {
        Reignited();
    }

}
