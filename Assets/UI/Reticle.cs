using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Reticle : MonoBehaviour
{

    public RectTransform RT;
    public Player player;

    public CanvasScaler CS;

    public float ConstantSpinAngle;
    public float SpinAngle;
    public float SpinRate;

    public float DecreaseRate;

    public float IncreaseSize;
    public float OriginalSize;

    private float timeC;

    private float currentAngle;
    private float targetAngle;

    private int screenWidth;
    private int screenHeight;

    private float canvasScale;

    private void Start()
    {
        UnityEngine.Cursor.visible = false;
    }


    private void ResolutionChanged()
    {
        float scale = (screenWidth - 640) / (1920 - 640);
        //if (scale > 1)
        //{
        //    scale = 1;
        //}
        if (scale < 0.5f)
        {
            scale = 0.5f;
        }

        CS.scaleFactor = scale;
        canvasScale = scale;
    }

    public void Recoil()
    {
        targetAngle += SpinAngle;
        RT.sizeDelta = new(IncreaseSize, IncreaseSize);
        timeC = 0;
    }

    private void Update()
    {
        ResolutionCheckUpdate();

        if (player.IsDead)
        {
            RT.anchoredPosition = new(-100, -100);
            return;
        }

        RT.anchoredPosition = Input.mousePosition * (1f / canvasScale);
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

    private void ResolutionCheckUpdate()
    {
        if (screenWidth != Screen.width || screenHeight != Screen.height)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            ResolutionChanged();
        }
    }

}
