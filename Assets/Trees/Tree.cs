using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    public int FirePoints;

    private GameObject FireObj;

    //Entry
    public void SetOnFire(GameObject fireObj, int firePoints)
    {
        FireObj = fireObj;
        FirePoints = firePoints;
    }

    public void SetExtinguished()
    {
        Destroy(FireObj);
        Destroy(this);
    }


    public void AddFirePoints(int firePoints)
    {
        FirePoints += firePoints;
    }
    public void ReduceFirePoints(int firePoints)
    {
        FirePoints -= firePoints;
        if (FirePoints <= 0)
        {
            SetExtinguished();
        }
    }

    private void Update()
    {

    }

}
