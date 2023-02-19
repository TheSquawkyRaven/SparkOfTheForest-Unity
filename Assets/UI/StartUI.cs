using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    public static bool StartAgain = false;

    public GameObject[] enableOnStart;

    public RectTransform RT;

    private void Awake()
    {
        if (StartAgain)
        {
            StartGame();
        }
        else
        {
            RT.anchoredPosition = Vector3.zero;
            Time.timeScale = 0;
        }
    }

    private bool started;
    private Vector2 start;
    private float timeC;

    public void StartGame()
    {
        if (StartAgain)
        {
            RT.anchoredPosition = new(-1000, -1000);
            gameObject.SetActive(false);
        }
        started = true;;
        Time.timeScale = 1;
        for (int i = 0; i < enableOnStart.Length; i++)
        {
            enableOnStart[i].SetActive(true);
        }
    }


    private void Update()
    {
        if (!started)
        {
            return;
        }
        timeC += Time.deltaTime;
        float scale = timeC / 1f;
        RT.anchoredPosition = Vector2.Lerp(start, new(0, -1000), scale);
        if (scale > 1)
        {
            gameObject.SetActive(false);
        }
    }






}
