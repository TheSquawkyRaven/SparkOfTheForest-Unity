using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour
{

    public SpriteRenderer SR;
    public Sprite[] anims;
    public float speed;

    private int animIndex;
    private float speedC;

    private void Update()
    {
        speedC += Time.deltaTime;
        if (speedC > speed)
        {
            speedC = 0;
            animIndex++;
            if (animIndex == anims.Length)
            {
                animIndex = 0;
            }
            SR.sprite = anims[animIndex];
        }
    }

}
