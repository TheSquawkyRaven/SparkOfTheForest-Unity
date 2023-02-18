using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{

    public RectTransform RT;

    public float ConstantSpinAngle;
    public float SpinAngle;
    public float SpinRate;

    public float DecreaseRate;

    public float IncreaseSize;
    public float OriginalSize;

    private float timeC;

    private float currentAngle;
    private float targetAngle;

    private void Start()
    {
        Cursor.visible = false;
    }

    public void Recoil()
    {
        targetAngle += SpinAngle;
        RT.sizeDelta = new(IncreaseSize, IncreaseSize);
        timeC = 0;
    }

    private void Update()
    {
        RT.anchoredPosition = Input.mousePosition;
        targetAngle += ConstantSpinAngle * Time.deltaTime;

        currentAngle = Mathf.Lerp(currentAngle, targetAngle, SpinRate);
        RT.localRotation = Quaternion.Euler(0, 0, currentAngle);

        timeC += Time.deltaTime;
        float scale = timeC / DecreaseRate;
        if (scale > 1)
        {
            scale = 1;
        }
        float size = Mathf.Lerp(IncreaseSize, OriginalSize, scale);
        RT.sizeDelta = new(size, size);

    }

}
