using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirePointsUI : MonoBehaviour
{

    public Slider FirePointsSlider;
    public TextMeshProUGUI FirePointsText;

    public void SetMaxFirePoints(int maxFirePoints)
    {
        FirePointsSlider.maxValue = maxFirePoints;
    }

    public void SetFirePoints(int firePoints)
    {
        FirePointsSlider.value = firePoints;
        FirePointsText.SetText(firePoints.ToString());
    }



}
