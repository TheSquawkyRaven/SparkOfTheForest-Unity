using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireLost : MonoBehaviour
{

    public static int treesDestroyed = 0;

    public Animator Animator;

    public TextMeshProUGUI TreesDestroyedText;
    public TextMeshProUGUI HighScoreText;

    private void Awake()
    {
        treesDestroyed = 0;
    }

    public void Lose()
    {
        Animator.SetTrigger("Lose");

        int highScore = PlayerPrefs.GetInt("highScore");

        if (treesDestroyed > highScore)
        {
            highScore = treesDestroyed;
            PlayerPrefs.SetInt("highScore", highScore);
        }

        TreesDestroyedText.SetText(treesDestroyed.ToString());
        HighScoreText.SetText(highScore.ToString());
    }

    public void Restart()
    {
        StartUI.StartAgain = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
