using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUI : MonoBehaviour
{


    public GameObject RightClick;
    public GameObject LeftClick;
    public GameObject Space;

    public void ShowPlaceCampfire(bool show)
    {
        Space.SetActive(show);
    }

    public void ShowDepositToCampfire(bool show)
    {
        RightClick.SetActive(show);
    }

}
