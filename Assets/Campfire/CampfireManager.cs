using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CampfireManager : MonoBehaviour
{

    //Returns remaining
    public int GiveFirePointsToCampfire(GameObject campfireObj, int firePoints)
    {
        Campfire campfire = campfireObj.GetComponent<Campfire>();
        return campfire.DepositFirePoints(firePoints);
    }


}
